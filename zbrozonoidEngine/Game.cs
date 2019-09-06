﻿/*
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
        public IGameConfig GameConfig { get; set; } = new GameConfig();

        public List<IBrick> BricksHitList { get; private set; } = new List<IBrick>();

        public int PadCurrentSpeed { get; private set; }

        public Game(int number)
        {
            GameConfig.Mouses = number;

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
            ballStateMachine = new BallStateMachine(this);
           
            OnLostBallsEvent += OnLostBalls;
        }

        public void Initialize()
        {
            InitializeLevel(true);
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

        public void Action()
        {
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
                InitializeLevel(false);
            }
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

        public void HandleBrickCollision(IBall currentBall, List<BrickHit> bricksHit)
        {
            BricksHitList = GetBricksHit(bricksHit);

            if (HitBrick(BricksHitList, out BrickType type))
            {
                BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit[0].Number);
                OnBrickHit?.Invoke(this, brickHitArgs);

                --levelManager.GetCurrent().BeatableBricksNumber;
                gameState.Scores++;

                ExecuteAdditionalEffect(currentBall, type);
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

        private void ExecuteAdditionalEffect(IBall currentBall, BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IPad pad = padManager.GetFirst();

                        IBall ball1 = CreateBallFactory();
                        padManager.SetBallStartPosition(pad, ball1);
                        ballManager.Add(ball1);

                        IBall ball2 = CreateBallFactory();
                        padManager.SetBallStartPosition(pad, ball2);
                        ballManager.Add(ball2);
                        break;
                    }
                case BrickType.DestroyerBall:
                    {
                        ITail tail = new Tail
                        {
                            FireBallTimerCallback = FireBallTimerHandler
                        };

                        tailManager.Add(currentBall, tail);
                        break;
                    }

                default:
                    break;
            }
        }

        public void SetPadMove(int delta, uint manipulator)
        {
            PadCurrentSpeed = delta;

            foreach (var value in padManager)
            {
                if (value.Item2 == manipulator)
                {
                    IPad pad = value.Item3;

                    pad.Boundary.Min = new Vector2(pad.Boundary.Min.X + PadCurrentSpeed, pad.Boundary.Min.Y);

                    screenCollisionManager.DetectAndVerify(pad);
                    VerifyBorderCollision(pad);
                }
            }
        }

        public void StartPlay()
        {
            if (!ballStateMachine.IsBallInIdleState())
            {
                return;
            }

            gameState.Pause = false;
            gameState.BallsOutOfScreen = 0;

            if (gameState.Lifes < 0)
            {
                gameState.Lifes = 3;
                gameState.Scores = 0;

                InitializeLevel(true);
            } 
            else
            {
                InitBall();
            }

            ballStateMachine.GoIntoPlay();
        }

        private void InitializeLevel(bool restartLevel)
        {
            CreateObjects();
            InitBall();

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

        private void CreateObjects()
        {
            padManager.Create(GameConfig);
            borderManager.Create(screen, GameConfig);

            ballManager.Clear();
            for (int i = 0; i < GameConfig.Players && i < GameConfig.Mouses; ++i)
            {
                ballManager.Add(CreateBallFactory());
            }

            foreach (var pad in padManager)
            {
                VerifyBorderCollision(pad.Item3);
            }
        }

        public void InitBall()
        {
            tailManager.Clear();

            var iterator = padManager.GetEnumerator();
            iterator.MoveNext();
            foreach (var ball in ballManager)
            {
                IPad pad = iterator.Current.Item3;
                padManager.SetBallStartPosition(pad, ball);

                iterator.MoveNext();
            }
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

        public void GameIsOver()
        {
            GameState.Pause = false;
            ballStateMachine.GoIntoIdle();
            InitBall();
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
            if (GameState.FasterBallCountdown.ContainsKey(ball))
            {
                if (value <= 0)
                {
                    GameState.FasterBallCountdown.Remove(ball);
                    return;
                }

                GameState.FasterBallCountdown[ball] = value;
            }
            else
            {
                GameState.FasterBallCountdown.Add(ball, value);
            }
        }

        public void FireBallTimerHandler(ITail tail, int value)
        {
            if (GameState.FireBallCountdown.ContainsKey(tail))
            {
                if (value <= 0)
                {
                    TailManager.Remove(tail);
                    GameState.FireBallCountdown.Remove(tail);
                    return;
                }

                GameState.FireBallCountdown[tail] = value;
            }
            else
            {
                GameState.FireBallCountdown.Add(tail, value);
            }
        }
    }
}
