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
    using System.Linq;
    using Autofac;
    using NLog;
    using zbrozonoidEngine.Interfaces;
    using zbrozonoidEngine.Managers;

    public class Game : IGame
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<LevelEventArgs> OnChangeLevel;
        public event EventHandler<BrickHitEventArgs> OnBrickHit;
        public event EventHandler<EventArgs> OnLostBallsEvent;

        private int ScreenWidth = 1024;

        private int ScreenHeight = 768;

        private readonly IScreen screen;

        private readonly BallStateMachine ballStateMachine;
        private readonly IGameState gameState;

        public List<IBrick> Bricks { get; private set; } = new List<IBrick>(); 
        public string BackgroundPath => ManagerScope.Resolve<ILevelManager>()?.GetCurrent()?.BackgroundPath;
        public IGameState GameState => gameState;
        public IGameConfig GameConfig { get; set; } = new GameConfig();

        private readonly ManagerScopeFactory managerScopeFactory = new ManagerScopeFactory();
        public ILifetimeScope ManagerScope { get; set; }

        public bool ForceChangeLevel { get; set; }

        private ILevelManager levelManager;
        private IBallManager ballManager;
        private IPadManager padManager;
        private ITailManager tailManager;
        private IBorderManager borderManager;
        private ICollisionManager collisionManager;
        private IScreenCollisionManager screenCollisionManager;


        public Game(int number)
        {
            GameConfig.Mouses = number;

            screen = new Screen
                         {
                             Width = ScreenWidth,
                             Height = ScreenHeight
                         };

            ManagerScope = managerScopeFactory.Create(screen);

            levelManager = ManagerScope.Resolve<ILevelManager>();
            ballManager = ManagerScope.Resolve<IBallManager>();
            padManager = ManagerScope.Resolve<IPadManager>();
            tailManager = ManagerScope.Resolve<ITailManager>();
            borderManager = ManagerScope.Resolve<IBorderManager>();
            collisionManager = ManagerScope.Resolve<ICollisionManager>();
            screenCollisionManager = ManagerScope.Resolve<IScreenCollisionManager>();

            //levelManager = new LevelManager();
            //collisionManager = new CollisionManager();
            //screenCollisionManager = new ScreenCollisionManager(screen);
            //tailManager = new TailManager();
            //ballManager = new BallManager();
            //borderManager = new BorderManager();
            //padManager = new PadManager(screen);


            gameState = new GameState();
            ballStateMachine = new BallStateMachine(ManagerScope, Bricks, SavePosition, HandleBrickCollision, LostBalls);
           
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
            //Logger.Info($"Pad position {posx}, {posy}");
        }

        public void GetPadSize(IPad pad, out int width, out int height)
        {
            pad.GetSize(out width, out height);
        }

        public void Action()
        {
            foreach (IBall ball in ballManager.ToList())
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

            if (levelManager.VerifyAllBricksAreHit() || ForceChangeLevel)
            {
                ForceChangeLevel = false;
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

        public void HandleBrickCollision(IBall currentBall, IEnumerable<int> bricksHit)
        {
            if (HitBrick(bricksHit, out BrickType type))
            {
                BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit.First());
                OnBrickHit?.Invoke(this, brickHitArgs);

                --levelManager.GetCurrent().BeatableBricksNumber;
                gameState.Scores++;

                ExecuteAdditionalEffect(currentBall, type);
            }
        }

        private bool HitBrick(IEnumerable<int> bricksHitList, out BrickType type)
        {
            type = BrickType.None;

            var bricksFound = Bricks.FilterByIndex(bricksHitList).Where(x => x.IsBeatable && x.IsVisible);
            if (bricksFound.Any())
            {
                bricksFound.First().IsHit = true;
                type = bricksFound.First().Type;

                return true;
            }
            return false;
        }

        private void ExecuteAdditionalEffect(IBall currentBall, BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IPad pad = ballManager.GetPadAssignedToBall(currentBall);

                        IBall ball1 = CreateBallFactory();
                        padManager.SetBallStartPosition(pad, ball1);
                        ballManager.Add(ball1, pad);

                        IBall ball2 = CreateBallFactory();
                        padManager.SetBallStartPosition(pad, ball2);
                        ballManager.Add(ball2, pad);
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

            foreach (var value in padManager.Where(x => x.Item2 == manipulator))
            {
                IPad pad = value.Item3;

                pad.Speed = delta;

                pad.Boundary.Min = new Vector2(pad.Boundary.Min.X + pad.Speed, pad.Boundary.Min.Y);

                screenCollisionManager.DetectAndVerify(pad);
                VerifyBorderCollision(pad);
            }
        }

        public void StartPlay()
        {
            if (!ballStateMachine.IsBallInIdleState())
            {
                return;
            }

            gameState.Pause = false;

            if (gameState.Lifes < 0)
            {
                gameState.Lifes = 3;
                gameState.Scores = 0;

                InitializeLevel(true);
            } 
            else
            {
                CreateBalls();
                InitBalls();
            }

            ballStateMachine.GoIntoPlay();
        }

        private void InitializeLevel(bool restartLevel)
        {
            CreateObjects();
            CreateLevelMap(restartLevel);

            LevelEventArgs background = new LevelEventArgs(levelManager.GetCurrent().BackgroundPath);
            OnChangeLevel?.Invoke(this, background);
        }

        private void CreateObjects()
        {
            padManager.Create(GameConfig);
            borderManager.Create(screen, GameConfig);

            foreach (var pad in padManager)
            {
                VerifyBorderCollision(pad.Item3);
            }

            CreateBalls();
            InitBalls();
        }

        private void CreateLevelMap(bool restartLevel)
        {
            if (restartLevel)
            {
                levelManager.Restart();
            }
            else
            {
                levelManager.MoveNext();
                levelManager.Load();
            }

            Bricks.Clear();
            Bricks.AddRange(levelManager.GetCurrent().Bricks);
        }

        private void CreateBalls()
        {
            ballManager.Clear();
            var padIterator = padManager.GetEnumerator();
            for (int i = 0; i < GameConfig.Players && i < GameConfig.Mouses; ++i)
            {
                padIterator.MoveNext();
                ballManager.Add(CreateBallFactory(), padIterator.Current.Item3 );
            }
        }

        public void InitBalls()
        {
            tailManager.Clear();

            foreach (var ball in ballManager)
            {
                IPad pad = ballManager.GetPadAssignedToBall(ball);
                padManager.SetBallStartPosition(pad, ball);
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
            if (value <= 0)
            {
                GameState.FasterBallCountdown.Remove(ball);
                return;
            }

            GameState.FasterBallCountdown[ball] = value;
        }

        public void FireBallTimerHandler(ITail tail, int value)
        {
            if (value <= 0)
            {
                if (tailManager.Remove(tail))
                {
                    GameState.FireBallCountdown.Remove(tail);
                }
                return;
            }

            GameState.FireBallCountdown[tail] = value;
        }
    }
}
