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

        private Mock<IRandomGenerator> generatorMock;
        private Mock<IMovement> movementMock;

        private IBall ball;
        private List<IBrick> bricks;
        private List<BrickWithNumber> bricksWithNumbers;
        private List<BrickWithNumber> bricksHit;
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
            ball.SetSize(15, 15);

            bricks = new List<IBrick>();
            bricksWithNumbers = new List<BrickWithNumber>();
            bricksHit = new List<BrickWithNumber>();
            manager = new CollisionManager();
            collisionState = new BallCollisionState();

            bricks.Add(new Brick(BrickType.Solid, 0, 0));
            bricks.Add(new Brick(BrickType.Solid, 50, 0));
            bricks.Add(new Brick(BrickType.Solid, 100, 0));
            bricks.Add(new Brick(BrickType.Solid, 100, 25));
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

            IHandleCollisionCommand command = new HandleBrickCollisionCommand(bricksWithNumbers, levelManagerMock.Object, null, manager, collisionState);
            command.Execute(ball);

            Assert.AreEqual(0, bricksHit.Count);
            Assert.IsFalse(collisionState.CollisionWithBrick);
            Assert.IsFalse(collisionState.BounceFromBrick);
        }

        [Test]
        public void VerifyBallShouldBounceFromOneBrick()
        {
            ball.Boundary.Min = new Vector2(0, 20);

            IHandleCollisionCommand command = new HandleBrickCollisionCommand(bricksWithNumbers, levelManagerMock.Object, null, manager,collisionState);
            command.Execute(ball);

            Assert.AreEqual(1, bricksHit.Count);
            Assert.IsTrue(collisionState.CollisionWithBrick);
            Assert.IsTrue(collisionState.BounceFromBrick);
        }


        [Test]
        public void VerifyBallShouldBounceFromTwoHorizontalBricks()
        {
            ball.Boundary.Min = new Vector2(45, 20);

            IHandleCollisionCommand command = new HandleBrickCollisionCommand(bricksWithNumbers, levelManagerMock.Object, null, manager, collisionState);
            command.Execute(ball);

            Assert.AreEqual(2, bricksHit.Count);
            Assert.IsTrue(collisionState.CollisionWithBrick);
            Assert.IsTrue(collisionState.BounceFromBrick);
        }




    }
}
