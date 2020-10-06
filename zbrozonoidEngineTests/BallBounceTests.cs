using System.Collections.Generic;
using Moq;
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;
using zbrozonoidEngine.States;
using zbrozonoidEngine.States.BallInPlayCommands;

namespace zbrozonoidEngineTests
{
    public class BounceTests
    {
        private const int Width = 1024;
        private const int Height = 800;
        private const int BorderSize = 12;
        private const int BallSize = 15;

        private Mock<IRandomGenerator> generatorMock;
        private Mock<IMovement> movementMock;
        private Mock<ILevelManager> levelManagerMock;

        private IBall ball;

        private readonly IScreen screen = new Screen() { Width = Width, Height = Height };

        private ICollisionManager manager;
        private BallCollisionState collisionState;

        [SetUp]
        public void Setup()
        {
            SetupNLog();

            generatorMock = new Mock<IRandomGenerator>();
            movementMock = new Mock<IMovement>();
            levelManagerMock = new Mock<ILevelManager>();

            ball = new Ball(generatorMock.Object, movementMock.Object);
            ball.SetSize(BallSize, BallSize);

            manager = new CollisionManager();
            collisionState = new BallCollisionState();
        }

        private void SetupNLog()
        {
            LogManager.Configuration = new LoggingConfiguration();
            var configuration = new LoggingConfiguration();
            var memoryTarget = new MemoryTarget { Name = "mem" };
            configuration.AddTarget(memoryTarget);
            configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, memoryTarget));
            LogManager.Configuration = configuration;
        }

        [Test]
        public void VerifyBallShouldNOTBounceFromOneBrick()
        {
            var bricks = CreateBricks(0, 0);
            ball.Boundary.Min = new Vector2(0, 50);

            ICollisionCommand command = new BrickCollisionCommand(bricks, levelManagerMock.Object, null, manager, collisionState);
            command.Detect(ball);

            Assert.AreEqual(0, collisionState.BricksHitList.Count);
            Assert.IsFalse(collisionState.CollisionWithBrick);
            Assert.IsFalse(collisionState.BounceFromBrick);
        }

        [Test]
        public void VerifyBallShouldBounceFromOneBrick()
        {
            var bricks = CreateBricks(0, 0);

            ball.Boundary.Min = new Vector2(0, 20);

            ICollisionCommand command = new BrickCollisionCommand(bricks, levelManagerMock.Object, null, manager,collisionState);
            command.Detect(ball);

            Assert.AreEqual(1, collisionState.BricksHitList.Count);
            Assert.IsTrue(collisionState.CollisionWithBrick);
            Assert.IsTrue(collisionState.BounceFromBrick);
        }


        [Test]
        public void VerifyBallShouldBounceFromTwoHorizontalBricks()
        {
            var bricks = CreateBricks(0, 0);

            ball.Boundary.Min = new Vector2(45, 20);

            ICollisionCommand command = new BrickCollisionCommand(bricks, levelManagerMock.Object, null, manager, collisionState);
            command.Detect(ball);

            Assert.AreEqual(2, collisionState.BricksHitList.Count);
            Assert.IsTrue(collisionState.CollisionWithBrick);
            Assert.IsTrue(collisionState.BounceFromBrick);
        }

        [Test]
        public void VerifyBallShouldBounceFromBorders()
        {
            var borders = CreateBorders();

            ball.Boundary.Min = new Vector2(Width - BorderSize - (BallSize - 1), Height - BorderSize - (BallSize - 1));
            
            collisionState.SetBorderCollistionState(true, true, borders);
            ICollisionCommand command = new BorderCollisionCommand(borders, manager, collisionState);
            command.Detect(ball);
            command.Bounce(ball);

            Assert.AreEqual(2, collisionState.BordersHitList.Count);
            Assert.IsTrue(collisionState.CollisionWithBorder);
            Assert.IsTrue(collisionState.BounceFromBorder);
        }

        private List<IBrick> CreateBricks(int posx, int posy)
        {
            var bricks = new List<IBrick>();

            bricks.Add(new Brick(BrickType.Solid, posx, posy));
            bricks.Add(new Brick(BrickType.Solid, posx + 50, posy + 0));
            bricks.Add(new Brick(BrickType.Solid, posx + 100, posy + 0));
            bricks.Add(new Brick(BrickType.Solid, posx + 100, posy + 25));

            return bricks;
        }

        private List<IBorder> CreateBorders()
        {
            var borders  = new List<IBorder>();

            borders.Add(new Border(screen, Edge.Top));
            borders.Add(new Border(screen, Edge.Left));
            borders.Add(new Border(screen, Edge.Right));
            borders.Add(new Border(screen, Edge.Bottom));

            return borders;
        }
    }
}
