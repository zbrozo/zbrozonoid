using Moq;
using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngineTests
{
    using NLog;
    using NLog.Config;
    using NLog.Targets;

    public class EngineTests
    {
        private Mock<IRandomGenerator> generatorMock;
        private Mock<IMovement> movementMock;

        [SetUp]
        public void Setup()
        {
            SetupNLog();

            generatorMock = new Mock<IRandomGenerator>();
            movementMock = new Mock<IMovement>();
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
        public void VerifyBallStartPosition()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);

            // Then
            Assert.AreEqual(0, ball.Boundary.Min.X);
            Assert.AreEqual(0, ball.Boundary.Min.Y);
        }

        [Test]
        public void VerifyBallMovement()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);

            // When
            ball.MoveBall();

            // Then
            var position = new Vector2();
            movementMock.Verify(x => x.Move(out position), Times.Once);
        }

        [Test]
        public void VerifyBallReverseMovement()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);

            // When
            ball.MoveBall(true);

            // Then
            var position = new Vector2();
            movementMock.Verify(x => x.ReverseMove(out position), Times.Once);
        }

        [Test]
        public void VerifyBallSize()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);
            ball.SetSize(334, 400);

            // When
            ball.GetSize(out int width, out int height);

            // Then
            Assert.AreEqual(334, width);
            Assert.AreEqual(400, height);
        }

        [Test]
        public void VerifyCalculateNewDegreeWithCentreType()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object,movementMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Centre);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<DegreeType>()), Times.Never);
        }

        [Test]
        public void VerifyCalculateNewDegreeWithAverageType()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Average);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<DegreeType>()), Times.Once);
        }

        [Test]
        public void VerifyCalculateNewDegreeWithCornerType()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object, movementMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Corner);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<DegreeType>()), Times.Once);
        }

    }
}