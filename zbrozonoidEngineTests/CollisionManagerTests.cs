using Moq;
using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;
using zbrozonoidLibrary;

namespace zbrozonoidEngineTests
{
    public class CollisionManagerTests
    {
        private Mock<ILoggerBase> loggerMock;
        private Mock<IRandomGenerator> generatorMock;
        private Mock<IMovement> movementMock;

        IBall ball1;
        IBall ball2;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILoggerBase>();
            generatorMock = new Mock<IRandomGenerator>();
            movementMock = new Mock<IMovement>();

            Logger.Instance = loggerMock.Object;

            ball1 = new Ball(generatorMock.Object, movementMock.Object);
            ball1.SetSize(15, 15);

            ball2 = new Ball(generatorMock.Object, movementMock.Object);
            ball2.SetSize(15, 15);
        }

        [Test]
        public void VerifyCollisionInsideFull()
        {
            // Given
            ICollisionManager manager = new CollisionManager();

            // When
            manager.Detect(ball1, ball2);

            // Then
            CollisionFlags flags = manager.GetFlags();
            Assert.IsTrue(flags.OverlapInsideFull());
        }

    }
}