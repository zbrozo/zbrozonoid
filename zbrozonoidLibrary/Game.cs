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
namespace zbrozonoidLibrary
{
    using System;
    using System.Collections.Generic;

    using zbrozonoidLibrary.Interfaces;
    using zbrozonoidLibrary.Managers;

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

        private readonly IRandomGenerator randomGenerator;

        private readonly ITailManager tailManager;

        private readonly IPadManager padManager;

        private readonly BallStateMachine ballStateMachine;

        private readonly IGameState gameState;

        public ITailManager TailManager => tailManager;
        public IBorderManager BorderManager => borderManager;
        public IBallManager BallManager => ballManager;
        public IPadManager PadManager => padManager;
        public List<IBrick> Bricks => levelManager.GetCurrent().Bricks;
        public string BackgroundPath => levelManager.GetCurrent().BackgroundPath;
        public IGameState GameState => gameState;

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
            randomGenerator = new RandomGenerator();
            tailManager = new TailManager();
            ballManager = new BallManager();
            borderManager = new BorderManager();
            padManager = new PadManager(screen);
            gameState = new GameState();

            ballStateMachine = new BallStateMachine(this, screenCollisionManager, collisionManager, padManager, borderManager, levelManager);

            padManager.Add(Edge.Top);
            padManager.Add(Edge.Bottom);

            borderManager.Create(screen);

            foreach (var pad in padManager)
            {
                VerifyBorderCollision(pad);
            }

            IBall ball = new Ball(randomGenerator);
            ball.SetSize(15, 15);
            ballManager.Add(ball);

            OnLostBallsEvent += OnLostBalls;
        }

        public void Initialize()
        {
            InitializeNewLevel(true);
           // StartPlay();
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
            posx = 0;
            posy = 0;

			IElement padElement = pad as IElement;
			if (padElement == null)
			{
				return;
			}

            posx = padElement.PosX;
            posy = padElement.PosY;
        }

        public void GetPadSize(IPad pad, out int width, out int height)
        {
            pad.GetSize(out width, out height);
        }

        public void SetBallStartPosition(IPad pad, IBall ball)
        {
            Logger.Instance.Write("---SetStartPosition---");

            IElement ballElement = ball as IElement;
            IElement padElement = pad as IElement;

            if (ballElement == null || padElement == null)
            {
                return;
            }

            ballElement.PosX = padElement.PosX + padElement.Width / 2 - ballElement.Width / 2;
            ballElement.PosY = padElement.PosY + padElement.Height;
            
            ball.OffsetX = ballElement.PosX;
            ball.OffsetY = ballElement.PosY;

            ball.SavedPosX = ballElement.PosX;
            ball.SavedPosY = ballElement.PosY;

            ball.Iteration = 0;
        }

        public void RestartBallYPosition(IPad pad, IBall ball)
        {
            Logger.Instance.Write("---RestartBallYPosition---");

            IElement ballElement = ball as IElement;
            IElement padElement = pad as IElement;

            if (ballElement == null || padElement == null)
            {
                return;
            }

            ballElement.PosY = padElement.PosY + padElement.Height;
            ball.OffsetY = ballElement.PosY;
            ball.SavedPosY = ballElement.PosY;
        }

        public void Action()
        {
            //uint ballsOutOfScreen = 0;
            foreach(IBall ball in ballManager)
            {
                //if (screenCollisionManager.Detect(ball))
                //{
                //    ++ballsOutOfScreen;
                //}

                int speed = ball.Speed;
                for (int i = 0; i < speed; ++i)
                {
                    if (!ballStateMachine.action(ball))
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

        private void InitializeNewLevel(bool restart)
        {
            ReinitBall();

            if (restart)
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
            collisionManager.bricksHit = GetBricksHit(bricksHit);

            if (collisionManager.HitBrick(out BrickType type))
            {
                BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit[0].Number);
                OnBrickHit?.Invoke(this, brickHitArgs);

                --levelManager.GetCurrent().BeatableBricksNumber;
                gameState.Scores++;

                ExecuteAdditionalEffect(type);
            }
        }

        private void ExecuteAdditionalEffect(BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IPad pad = padManager.GetFirst();

                        IBall ball1 = new Ball(randomGenerator);
                        ball1.SetSize(15, 15);
                        SetBallStartPosition(pad, ball1);
                        ballManager.Add(ball1);

                        IBall ball2 = new Ball(randomGenerator);
                        ball2.SetSize(15, 15);
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
            foreach (IPad pad in padManager)
            {
				    IElement padElement = pad as IElement;
				    if (padElement == null)
                {
                    return;
                }

                padElement.PosX += delta;

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
            if (gameState.ShouldGo)
            {
                return;
            }

            if (gameState.Lives < 0)
            {
                gameState.Lives = 3;
                gameState.Scores = 0;
                gameState.BallsOutOfScreen = 0;

                InitializeNewLevel(true);
            } 
            else
            {
                ReinitBall();
            }

            gameState.ShouldGo = true;
            gameState.BallsOutOfScreen = 0;
            ballStateMachine.goToInGame();
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

            IElement element = ball as IElement;
            if (ball != null)
            {
                ITail tail = tailManager.Find(ball);
                if (tail != null)
                {
                    Position position = new Position { X = element.PosX, Y = element.PosY };
                    tail.Add(position);
                }
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
            --GameState.Lives;
            gameState.ShouldGo = false;
            ballStateMachine.goToInMenu();
        }

    }
}
