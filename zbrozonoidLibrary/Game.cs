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
        public event EventHandler<BackgroundEventArgs> OnChangeBackground;

        private int padMovementSpeed = 10;

        private int ScreenWidth = 1024;

        private int ScreenHeight = 768;

        private readonly IScreen screen;

        private readonly IPad pad;

        private readonly ILevelManager levelManager;

        private readonly ICollisionManager collisionManager;

        private readonly ICollisionManager collisionManagerForMoveReversion;

        private readonly IBallManager ballManager;

        private readonly IBorderManager borderManager;

        private readonly IScreenCollisionManager screenCollisionManager;

        private readonly IRandomGenerator randomGenerator;

        public bool ShouldGo { get; set; }

        public int Lives { get; set; } = -1;

        public Game()
        {

            screen = new Screen
                         {
                             Width = ScreenWidth,
                             Height = ScreenHeight
                         };

            pad = new Pad();
            pad.SetSize(100, 24);

            levelManager = new LevelManager();
            collisionManager = new CollisionManager();
            collisionManagerForMoveReversion = new CollisionManager();
            screenCollisionManager = new ScreenCollisionManager(screen);
            randomGenerator = new RandomGenerator();

            ballManager = new BallManager();
            borderManager = new BorderManager();
            borderManager.Create(screen);
            VerifyBorderCollision(pad);

            IBall ball = new Ball(randomGenerator);
            ball.SetSize(15, 15);
            ballManager.Add(ball);
        }

        public void Initialize()
        {
            InitializeNewLevel();
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

        public void GetPadPosition(out int posx, out int posy)
        {
            posx = 0;
            posy = 0;
            if (!(pad is IElement padElement))
            {
                return;
            }
            posx = padElement.PosX;
            posy = padElement.PosY;
        }

        public IBallManager GetBallManager()
        {
            return ballManager;
        }

        public void GetPadSize(out int width, out int height)
        {
            pad.GetSize(out width, out height);
        }

        private void SetBallStartPosition(IBall ball)
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

        private void RestartBallYPosition(IBall ball)
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
            uint ballsOutOfScreen = 0;
            foreach(IBall ball in ballManager)
            {
                if (screenCollisionManager.Detect(ball))
                {
                    ++ballsOutOfScreen;
                    continue;
                }

                int speed = ball.Speed;
                for (int i = 0; i < speed; ++i)
                {
                    if (!DoAction(ball))
                    {
                        break;
                    }
                }
            }

            if (ballsOutOfScreen == ballManager.Count)
            {
                --Lives;
                ShouldGo = false;
            }

            if (levelManager.VerifyAllBricksAreHit())
            {
                InitializeNewLevel();
            }
        }

        private bool DoAction(IBall ball)
        {
            if (ShouldGo)
            {
                ball.MoveBall();
            }

            if (screenCollisionManager.Detect(ball))
            {
                ball.SavePosition();
                return false;
            }

            bool borderHit = VerifyBorderCollision(ball);

            if (collisionManager.Detect(pad, ball))
            {
                pad.LogData();

                CorrectBallPosition(ball);
                collisionManager.Bounce(ball);

                ball.LogData();
                return false;
            }

            collisionManager.Prepare();
            bool result = DetectBrickCollision(ball);
            if (result)
            {
                if (collisionManager.HitBrick(out BrickType type))
                {
                    --levelManager.GetCurrent().BeatableBricksNumber;

                    ExecuteAdditionalEffect(type);
                }

                bool destroyerBall = ball.GetTail() != null;
                if (!borderHit && !destroyerBall)
                {
                    collisionManager.Bounce(ball);
                }
                return false;
            }

            ball.SavePosition();

            return true;
        }

        private void InitializeNewLevel()
        {
            ShouldGo = false;

            ReinitBall();

            levelManager.MoveNext();
            levelManager.Load();

            BackgroundEventArgs background = new BackgroundEventArgs(levelManager.GetCurrent().BackgroundPath);
            OnChangeBackground?.Invoke(this, background);
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

        private bool VerifyBorderCollision(IBall ball)
        {
            foreach(IBorder border in borderManager)
            {
                IBorderCollisionManager borderCollisionManager = new BorderCollisionManager(border, collisionManager);
                if (borderCollisionManager.DetectAndVerify(ball))
                {
                    return true;
                }
            }
            return false;
        }

        private void CorrectBallPosition(IBall ball)
        {
            while (collisionManagerForMoveReversion.Detect(pad, ball))
            {
                if (!ball.MoveBall(true))
                {
                    RestartBallYPosition(ball);
                    return;
                }

                ball.SavePosition();

                foreach(IBorder border in borderManager)
                {
                    if (collisionManagerForMoveReversion.Detect(border, ball))
                    {
                        SetBallStartPosition(ball);
                        break;
                    }
                }

                if (screenCollisionManager.DetectAndVerify(ball))
                {
                    SetBallStartPosition(ball);
                    break;
                }

            }
        }

        private bool DetectBrickCollision(IBall ball)
        {
            List<IBrick> bricks = levelManager.GetCurrent().Bricks;

            bool result = false;
            foreach (var value in bricks)
            {
                IBrick brick = value;
                if (brick.Hit || !brick.IsVisible())
                {
                    continue;
                }

                if (collisionManager.Detect(brick, ball))
                {
                    result = true;
                }
            }
            return result;
        }


        private void ExecuteAdditionalEffect(BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IBall ball1 = new Ball(randomGenerator);
                        ball1.SetSize(15, 15);
                        SetBallStartPosition(ball1);
                        ballManager.Add(ball1);

                        IBall ball2 = new Ball(randomGenerator);
                        ball2.SetSize(15, 15);
                        SetBallStartPosition(ball2);
                        ballManager.Add(ball2);
                        break;
                    }
                case BrickType.DestroyerBall:
                    {
                        foreach (IBall ball in ballManager)
                        {
                            ITail tail = new Tail();
                            ball.AddTail(tail);
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        public void SetPadMove(int delta)
        {
            if (!(pad is IElement padElement))
            {
                return;
            }
            padElement.PosX += delta;

            screenCollisionManager.DetectAndVerify(pad);
            VerifyBorderCollision(pad);
        }

        public void SetBallMove()
        {
            foreach (IBall ball in ballManager)
            {
                SetBallStartPosition(ball);
            }
        }

        public void SetPadMinPosition()
        {
            if (!(pad is IElement padElement))
            {
                return;
            }

            padElement.PosX = 0;
        }

        public void SetPadMaxPosition()
        {
            if (!(pad is IElement padElement))
            {
                return;
            }

            GetPadSize(out int width, out int heigth);
            GetScreenSize(out int screenWidth, out int screenHeigth);
            padElement.PosX = screenWidth - width;
        }

        public List<IBrick> GetBricks()
        {
            return levelManager.GetCurrent().Bricks;
        }

        public IBorderManager GetBorderManager()
        {
            return borderManager;
        }

        public string GetBackgroundPath()
        {
            return levelManager.GetCurrent().BackgroundPath;
        }

        public void StartPlay()
        {
            if (ShouldGo)
            {
                return;
            }

            if (Lives < 0)
            {
                Lives = 3;

                levelManager.Reset();
                levelManager.MoveNext();
                levelManager.Load();
            }

            ReinitBall();

            ShouldGo = true;
        }
        private void ReinitBall()
        {
            ballManager.LeaveOnlyOne();

            IBall ball = ballManager.GetFirst();
            if (ball is null)
            {
                return;
            }

            SetBallStartPosition(ball);
        }

    }
}
