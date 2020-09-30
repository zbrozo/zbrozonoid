using System;
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
        private const int width = 1024;
        private const int height = 800;
        private const int borderSize = 12;
        private const int ballSize = 15;

        private Mock<IRandomGenerator> generatorMock;
        private Mock<IMovement> movementMock;
        private IBall ball;
        private IScreen screen = new Screen() { Width = width, Height = height };
        private List<IBrick> bricks;
        private List<IBorder> borders;
        private ICollisionManager manager;
        private BallCollisionState collisionState;
        private Mock<ILevelManager> levelManagerMock;

        [SetUp]
        public void Setup()
        {
            SetupNLog();

            generatorMock = new Mock<IRandomGenerator>();
            movementMock = new Mock<IMovement>();
            levelManagerMock = new Mock<ILevelManager>();

            ball = new Ball(generatorMock.Object, movementMock.Object);
            ball.SetSize(ballSize, ballSize);

            bricks = new List<IBrick>();
            borders = new List<IBorder>();
            manager = new CollisionManager();
            collisionState = new BallCollisionState();

            bricks.Add(new Brick(BrickType.Solid, 0, 0));
            bricks.Add(new Brick(BrickType.Solid, 50, 0));
            bricks.Add(new Brick(BrickType.Solid, 100, 0));
            bricks.Add(new Brick(BrickType.Solid, 100, 25));

            borders.Add(new Border(screen, Edge.Right));
            borders.Add(new Border(screen, Edge.Bottom));

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
            ball.Boundary.Min = new Vector2(width - borderSize - (ballSize -1), height - borderSize - (ballSize - 1));

            ICollisionCommand command = new BorderCollisionCommand(borders, manager, collisionState);
            command.Detect(ball);

            Assert.AreEqual(2, collisionState.BordersHitList.Count);
            Assert.IsTrue(collisionState.CollisionWithBorder);
            Assert.IsTrue(collisionState.BounceFromBorder);
        }

    }
}
