using Moq;
using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngineTests
{
    public class EngineTests
    {
        private Mock<ILoggerBase> loggerMock;
        private Mock<IRandomGenerator> generatorMock;

        [SetUp]
        public void Setup()
        {
            loggerMock = new Mock<ILoggerBase>();
            generatorMock = new Mock<IRandomGenerator>();

            Logger.Instance = loggerMock.Object;
        }

        [Test]
        public void BallStartPosition()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);

            // Then
            Assert.AreEqual(0, ball.Boundary.Min.X);
            Assert.AreEqual(0, ball.Boundary.Min.Y);
        }

        [Test]
        public void BallMovement()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);

            // When
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();
            ball.MoveBall();

            // Then
            Assert.AreEqual(7, ball.Boundary.Min.X);
            Assert.AreEqual(7, ball.Boundary.Min.Y);
            loggerMock.Verify(x => x.Write(It.IsAny<string>()), Times.Exactly(10));
        }
    }
}
