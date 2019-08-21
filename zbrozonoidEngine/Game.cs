/*
Copyright(C) 2018 Tomasz Zbrożek

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see<https://www.gnu.org/licenses/>.
*/
namespace zbrozonoidEngine
{
    using System;
    using System.Collections.Generic;

    using zbrozonoidEngine.Interfaces;
    using zbrozonoidEngine.Managers;

    public class Game : IGame
    {
        public event EventHandler<LevelEventArgs> OnChangeLevel;
        public event EventHandler<BrickHitEventArgs> OnBrickHit;
        public event EventHandler<EventArgs> OnLostBallsEvent;

        private int ScreenWidth = 1024;

        private int ScreenHeight = 768;

        private readonly IScreen screen;

        private readonly ILevelManager levelManager;

        private readonly ICollisionManager collisionManager;

        private readonly IBallManager ballManager;

        private readonly IBorderManager borderManager;

        private readonly IScreenCollisionManager screenCollisionManager;

        private readonly ITailManager tailManager;

        private readonly IPadManager padManager;

        private readonly BallStateMachine ballStateMachine;

        private readonly IGameState gameState;

        public ILevelManager LevelManager => levelManager;
        public ICollisionManager CollisionManager => collisionManager;
        public IScreenCollisionManager ScreenCollisionManager => screenCollisionManager;
        public ITailManager TailManager => tailManager;
        public IBorderManager BorderManager => borderManager;
        public IBallManager BallManager => ballManager;
        public IPadManager PadManager => padManager;
        public List<IBrick> Bricks => levelManager.GetCurrent().Bricks;
        public string BackgroundPath => levelManager.GetCurrent().BackgroundPath;
        public IGameState GameState => gameState;

        public List<IBrick> BricksHitList = new List<IBrick>();

        public int PadCurrentSpeed { get; private set; }

        public Game()
        {

            screen = new Screen
                         {
                             Width = ScreenWidth,
                             Height = ScreenHeight
                         };

            levelManager = new LevelManager();
            collisionManager = new CollisionManager();
            screenCollisionManager = new ScreenCollisionManager(screen);
            tailManager = new TailManager();
            ballManager = new BallManager();
            borderManager = new BorderManager();
            padManager = new PadManager(screen);
            gameState = new GameState();

            ballStateMachine = new BallStateMachine(this, padManager, borderManager, levelManager);

            padManager.Add(Edge.Top);
            padManager.Add(Edge.Bottom);

            borderManager.Create(screen);

            foreach (var pad in padManager)
            {
                VerifyBorderCollision(pad);
            }

            ballManager.Add(CreateBallFactory());

            OnLostBallsEvent += OnLostBalls;
        }

        public void Initialize()
        {
            InitializeNewLevel(true);
        }

        public void SetScreenSize(int width, int height)
        {
            screen.Width = width;
            screen.Height = height;
        }

        public void GetScreenSize(out int width, out int height)
        {
            width = ScreenWidth;
            height = ScreenHeight;
        }

        public void GetPadPosition(IPad pad, out int posx, out int posy)
        {
            posx = pad.Boundary.Min.X;
            posy = pad.Boundary.Min.Y;
            Logger.Instance.Write($"Pad position {posx}, {posy}");
        }

        public void GetPadSize(IPad pad, out int width, out int height)
        {
            pad.GetSize(out width, out height);
        }

        public void SetBallStartPosition(IPad pad, IBall ball)
        {
            Logger.Instance.Write("---SetStartPosition---");

            int x = pad.Boundary.Min.X + pad.Boundary.Size.X / 2 - ball.Boundary.Size.X / 2;
            int y = pad.Boundary.Max.Y;
            ball.Boundary.Min = new Vector2(x, y);
            ball.InitStartPosition();
        }

        public void RestartBallYPosition(IPad pad, IBall ball)
        {
            Logger.Instance.Write("---RestartBallYPosition---");
            ball.SetYPosition(pad.Boundary.Max.Y);
        }

        public void Action()
        {
            GameState.BallsOutOfScreen = 0;

            foreach (IBall ball in ballManager)
            {
                int speed = ball.Speed;
                for (int i = 0; i < speed; ++i)
                {
                    if (!ballStateMachine.Action(ball))
                    {
                        break;
                    }
                }
            }

            if (levelManager.VerifyAllBricksAreHit())
            {
                InitializeNewLevel(false);
            }
        }

        private void InitializeNewLevel(bool restartLevel)
        {
            ReinitBall();

            if (restartLevel)
            {
                levelManager.Restart();
            }
            else
            {
                levelManager.MoveNext();
                levelManager.Load();
            }

            LevelEventArgs background = new LevelEventArgs(levelManager.GetCurrent().BackgroundPath);
            OnChangeLevel?.Invoke(this, background);
        }


        private void VerifyBorderCollision(IPad pad)
        {
            foreach(IBorder border in borderManager)
            {
                IBorderCollisionManager borderCollisionManager = new BorderCollisionManager(border, collisionManager);
                if (borderCollisionManager.DetectAndVerify(pad))
                {
                    break;
                }
            }
        }

        public void HandleBrickCollision(List<BrickHit> bricksHit)
        {
            BricksHitList = GetBricksHit(bricksHit);

            if (HitBrick(BricksHitList, out BrickType type))
            {
                BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit[0].Number);
                OnBrickHit?.Invoke(this, brickHitArgs);

                --levelManager.GetCurrent().BeatableBricksNumber;
                gameState.Scores++;

                ExecuteAdditionalEffect(type);
            }
        }

        private bool HitBrick(List<IBrick> bricksHit, out BrickType type)
        {
            type = BrickType.None;
            if (bricksHit.Count != 0)
            {
                foreach (var brick in bricksHit)
                {
                    if (brick.IsBeatable() && brick.IsVisible())
                    {
                        brick.Hit = true;
                        type = brick.Type;
                        return true;
                    }
                }
            }
            return false;
        }

        private void ExecuteAdditionalEffect(BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IPad pad = padManager.GetFirst();

                        IBall ball1 = CreateBallFactory();
                        SetBallStartPosition(pad, ball1);
                        ballManager.Add(ball1);

                        IBall ball2 = CreateBallFactory();
                        SetBallStartPosition(pad, ball2);
                        ballManager.Add(ball2);
                        break;
                    }
                case BrickType.DestroyerBall:
                    {
                        foreach (IBall ball in ballManager)
                        {
                            tailManager.Add(ball);
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        public void SetPadMove(int delta)
        {
            PadCurrentSpeed = delta;

            foreach (IPad pad in padManager)
            {
                pad.Boundary.Min = new Vector2(pad.Boundary.Min.X + delta, pad.Boundary.Min.Y);

                screenCollisionManager.DetectAndVerify(pad);
                VerifyBorderCollision(pad);
            }
        }

        public void SetBallMove()
        {
            foreach (IBall ball in ballManager)
            {
                IPad pad = padManager.GetFirst();
                SetBallStartPosition(pad, ball);
            }
        }

        public void StartPlay()
        {
            if (!ballStateMachine.IsBallInIdleState())
            {
                return;
            }

            if (gameState.Lifes < 0)
            {
                gameState.Lifes = 3;
                gameState.Scores = 0;
                gameState.BallsOutOfScreen = 0;

                InitializeNewLevel(true);
            } 
            else
            {
                ReinitBall();
            }

            gameState.BallsOutOfScreen = 0;
            ballStateMachine.GoIntoPlay();
        }

        private void ReinitBall()
        {
            tailManager.Clear();

            ballManager.LeaveOnlyOne();

            IBall ball = ballManager.GetFirst();
            if (ball == null)
            {
                return;
            }
            ball.GoDefaultSpeed();

            IPad pad = padManager.GetFirst();
            SetBallStartPosition(pad, ball);
        }

        public bool IsBallDestroyer(IBall ball)
        {
            return tailManager.Find(ball) != null;
        }

        public void SavePosition(IBall ball)
        {
            ball.SavePosition();

            ITail tail = tailManager.Find(ball);
            if (tail != null)
            {
                Vector2 position = new Vector2 ( ball.Boundary.Min.X, ball.Boundary.Min.Y );
                tail.Add(position);
            }
        }

        private List<IBrick> GetBricksHit(List<BrickHit> bricksHit)
        {
            List<IBrick> bricks = new List<IBrick>();
            foreach (var value in bricksHit)
            {
                bricks.Add(value.Brick);
            }
            return bricks;
        }

        public void LostBalls()
        {
            OnLostBallsEvent?.Invoke(this, null);
        }

        public void OnLostBalls(object sender, EventArgs args)
        {
            --GameState.Lifes;
            ballStateMachine.GoIntoIdle();
        }

        public IBall CreateBallFactory()
        {
            const int defaultDegree = 45;
            var defaultDirection = new Vector2(1, 1);
            var defaultOffset = new Vector2(0, 0);
            IBall ball = new Ball(new RandomGenerator(),
                                  new LinearMovement(0, defaultDegree, defaultOffset, defaultDirection)
                            )
            {
                BallSpeedTimerCallback = BallSpeedTimerHandler
            };

            ball.SetSize(15, 15);
            return ball;
        }

        public void BallSpeedTimerHandler(IBall ball, int value)
        {
            if (GameState.BallFasterCountdown.ContainsKey(ball))
            {
                if (value <= 0)
                {
                    GameState.BallFasterCountdown.Remove(ball);
                    return;
                }

                GameState.BallFasterCountdown[ball] = value;
            }
            else
            {
                GameState.BallFasterCountdown.Add(ball, value);
            }
        }
    }
}
