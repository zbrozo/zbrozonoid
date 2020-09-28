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
    using zbrozonoidEngine.Counters;
    using zbrozonoidEngine.Interfaces;

    public class Game : IGame
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // external events 
        public event EventHandler<LevelEventArgs> OnChangeLevelEvent;
        public event EventHandler<BrickHitEventArgs> OnBrickHitEvent;
        public event EventHandler<EventArgs> OnLostBallEvent;

        private int ScreenWidth = 1024;

        private int ScreenHeight = 768;

        private readonly IScreen screen;

        private readonly BallStateMachine ballStateMachine;

        public ICollection<IBrick> Bricks { get; private set; } = new List<IBrick>();

        public FastBallCounter FastBallCounter { get; } = new FastBallCounter();
        public FireBallCounter FireBallCounter { get; private set; }
 
        public IGameState GameState { get; } = new GameState();
        public IGameConfig GameConfig { get; } = new GameConfig();

        private readonly ManagerScopeFactory managerScopeFactory = new ManagerScopeFactory();
        public ILifetimeScope ManagerScope { get; private set; }

        public bool ForceChangeLevel { get; set; }

        private BallFactory ballFactory;

        private LevelFactory levelFactory;

        private ILevelManager levelManager;
        private IBallManager ballManager;
        private IPadManager padManager;
        private ITailManager tailManager;
        private IBorderManager borderManager;
        private ICollisionManager collisionManager;
        private IScreenCollisionManager screenCollisionManager;
        private IBorderCollisionManager borderCollisionManager;



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
            borderCollisionManager = ManagerScope.Resolve<IBorderCollisionManager>();

            FireBallCounter = new FireBallCounter(tailManager);

            ballStateMachine = new BallStateMachine(ManagerScope, Bricks, SavePosition, HandleBrickCollision, LostBall);

            ballFactory = new BallFactory(ballManager, tailManager, padManager, GameConfig, FastBallCounter.TimerHandler);

            levelFactory = new LevelFactory(
                screen,
                levelManager,
                padManager,
                borderManager,
                borderCollisionManager,
                ballFactory,
                GameConfig,
                Bricks);
        }

        public void Initialize()
        {
            CreateLevel(false);
        }

        public void GetScreenSize(out int width, out int height)
        {
            width = ScreenWidth;
            height = ScreenHeight;
        }

        public void Action()
        {
            foreach (IBall ball in ballManager.ToArray())
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
                CreateLevel(false);
            }
        }

        private void HandleBrickCollision(IBall currentBall, IEnumerable<int> bricksHit)
        {
            if (HitBrick(bricksHit, out BrickType type))
            {
                BrickHitEventArgs brickHitArgs = new BrickHitEventArgs(bricksHit.First());
                OnBrickHitEvent?.Invoke(this, brickHitArgs);

                --levelManager.GetCurrent().BeatableBricksNumber;
                GameState.Scores++;

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
                        ballFactory.CreateBall(pad);
                        ballFactory.CreateBall(pad);
                        break;
                    }
                case BrickType.DestroyerBall:
                    {
                        ITail tail = new Tail { FireBallTimerCallback = FireBallCounter.FireBallTimerHandler };
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
                borderCollisionManager.DetectAndVerify(borderManager, pad);
            }
        }

        public void StartPlay()
        {
            if (!ballStateMachine.IsBallInIdleState())
            {
                return;
            }

            GameState.Pause = false;

            if (GameState.Lifes < 0)
            {
                GameState.Lifes = 3;
                GameState.Scores = 0;

                CreateLevel(true);
            }
            else
            {
                ballFactory.CreateBalls();
            }

            ballStateMachine.GoIntoPlay();
        }

        private void LostBall()
        {
            --GameState.Lifes;
            ballStateMachine.GoIntoIdle();

            OnLostBallEvent?.Invoke(this, null);
        }

        public void GameIsOver()
        {
            GameState.Pause = false;
            ballStateMachine.GoIntoIdle();
        }

        private void SavePosition(IBall ball)
        {
            ball.SavePosition();

            ITail tail = tailManager.Find(ball);
            if (tail != null)
            {
                Vector2 position = new Vector2(ball.Boundary.Min.X, ball.Boundary.Min.Y);
                tail.Add(position);
            }
        }

        private void CreateLevel(bool restartLevel)
        {
            levelFactory.Create(restartLevel);
            var args = new LevelEventArgs(levelManager.GetCurrent().BackgroundPath);
            OnChangeLevelEvent?.Invoke(this, args);
        }

    }
}
