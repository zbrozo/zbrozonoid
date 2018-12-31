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
        public event EventHandler<BrickHitEventArgs> OnBrickHit;

        private int ScreenWidth = 1024;

        private int ScreenHeight = 768;

        private readonly IScreen screen;

        private readonly ILevelManager levelManager;

        private readonly ICollisionManager collisionManager;

        private readonly ICollisionManager collisionManagerForMoveReversion;

        private readonly IBallManager ballManager;

        private readonly IBorderManager borderManager;

        private readonly IScreenCollisionManager screenCollisionManager;

        private readonly IRandomGenerator randomGenerator;

        private readonly ITailManager tailManager;

        private readonly IPadManager padManager;

        public bool ShouldGo { get; set; }

        public int Lives { get; set; } = -1;

        public int Scores { get; set; } = 0;

        public ITailManager TailManager => tailManager;
        public IBorderManager BorderManager => borderManager;
        public IBallManager BallManager => ballManager;
        public IPadManager PadManager => padManager;
        public List<IBrick> Bricks => levelManager.GetCurrent().Bricks;
        public string BackgroundPath => levelManager.GetCurrent().BackgroundPath;

        public class BrickHit
        {
            public BrickHit(int number, IBrick brick)
            {
                Number = number;
                Brick = brick;
            }

            public int Number { get; set; }
            public IBrick Brick { get; set; }
        }

        private readonly List<BrickHit> bricksHit = new List<BrickHit>();

        public Game()
        {

            screen = new Screen
                         {
                             Width = ScreenWidth,
                             Height = ScreenHeight
                         };

            levelManager = new LevelManager();
            collisionManager = new CollisionManager();
            collisionManagerForMoveReversion = new CollisionManager();
            screenCollisionManager = new ScreenCollisionManager(screen);
            randomGenerator = new RandomGenerator();
            tailManager = new TailManager();
            ballManager = new BallManager();
            borderManager = new BorderManager();
            padManager = new PadManager(screen);

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

        private void SetBallStartPosition(IPad pad, IBall ball)
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

        private void RestartBallYPosition(IPad pad, IBall ball)
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

            if (screenCollisionManager.DetectAndVerify(ball))
            {
                ball.SavePosition();
                return false;
            }

            collisionManager.bricksHit = null;

            bool borderHit = VerifyBorderCollision(ball);

            foreach (IPad pad in padManager)
            {
                if (collisionManager.Detect(pad, ball))
                {
                    pad.LogData();

                    CorrectBallPosition(pad, ball);
                    collisionManager.Bounce(ball);

                    ball.LogData();
                    return false;
                }
            }

            //collisionManager.Prepare();
            bool result = DetectBrickCollision(ball);
            if (result)
            {
                List<IBrick> bricks = new List<IBrick>();
                foreach (var value in bricksHit)
                {
                    bricks.Add(value.Brick);
                }
                collisionManager.bricksHit = bricks;

                if (collisionManager.HitBrick(out BrickType type))
                {
                    BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit[0].Number);
                    OnBrickHit?.Invoke(this, brickHitArgs);

                    --levelManager.GetCurrent().BeatableBricksNumber;
                    Scores++;

                    ExecuteAdditionalEffect(type);
                }

                bool destroyerBall = IsBallDestroyer(ball);
                if (!borderHit && !destroyerBall)
                {
                    collisionManager.Bounce(ball);
                }
                return false;
            }

            SavePosition(ball);

            return true;
        }

        private void InitializeNewLevel()
        {
            ShouldGo = true;

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

        private void CorrectBallPosition(IPad pad, IBall ball)
        {
            while (collisionManagerForMoveReversion.Detect(pad, ball))
            {
                if (!ball.MoveBall(true))
                {
                    RestartBallYPosition(pad, ball);
                    return;
                }

                ball.SavePosition();

                foreach(IBorder border in borderManager)
                {
                    if (collisionManagerForMoveReversion.Detect(border, ball))
                    {
                        SetBallStartPosition(pad, ball);
                        break;
                    }
                }

                if (screenCollisionManager.DetectAndVerify(ball))
                {
                    SetBallStartPosition(pad, ball);
                    break;
                }

            }
        }

        private bool DetectBrickCollision(IBall ball)
        {
            bricksHit.Clear();

            List<IBrick> bricks = levelManager.GetCurrent().Bricks;

            bool result = false;
            int id = 0;
            foreach (var value in bricks)
            {
                IBrick brick = value;
                if (brick.Hit || !brick.IsVisible())
                {
                    ++id;
                    continue;
                }

                if (collisionManager.Detect(brick, ball))
                {
                    bricksHit.Add(new BrickHit(id, brick));
                    result = true;
                }
                ++id;
            }
            return result;
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
            if (ShouldGo)
            {
                return;
            }

            if (Lives < 0)
            {
                Lives = 3;
                Scores = 0;

                levelManager.Restart();
            }

            ReinitBall();

            ShouldGo = true;
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

        private bool IsBallDestroyer(IBall ball)
        {
            return tailManager.Find(ball) != null;
        }

        private void SavePosition(IBall ball)
        {
            ball.SavePosition();

            IElement element = ball as IElement;
            if (ball == null)
            {
                ITail tail = tailManager.Find(ball);
				    if (tail != null)
                {
                    Position position = new Position { X = element.PosX, Y = element.PosY };
                    tail.Add(position);
                }
            }
        }
    }
}
