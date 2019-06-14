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
        public void VerifyBallStartPosition()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);

            // Then
            Assert.AreEqual(0, ball.Boundary.Min.X);
            Assert.AreEqual(0, ball.Boundary.Min.Y);
        }

        [Test]
        public void VerifyBallMovement()
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

        [Test]
        public void VerifyBallSize()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);
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
            IBall ball = new Ball(generatorMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Centre);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<int>(), It.IsAny<DegreeType>()), Times.Never);
        }

        [Test]
        public void VerifyCalculateNewDegreeWithAverageType()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Average);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<int>(), It.IsAny<DegreeType>()), Times.Once);
        }

        [Test]
        public void VerifyCalculateNewDegreeWithCornerType()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);

            // When
            ball.CalculateNewDegree(DegreeType.Corner);

            // Then
            generatorMock.Verify(x => x.GenerateDegree(It.IsAny<int>(), It.IsAny<DegreeType>()), Times.Once);
        }

        [Test]
        public void VerifyInitStartPosition()
        {
            // Given
            IBall ball = new Ball(generatorMock.Object);
            ball.Boundary.Min = new Vector2(20, 50);

            // When
            ball.InitStartPosition();

            // Then
            Assert.AreEqual(20, ball.OffsetX);
            Assert.AreEqual(50, ball.OffsetY);
            Assert.AreEqual(20, ball.SavedPosX);
            Assert.AreEqual(50, ball.SavedPosY);
            Assert.AreEqual(0, ball.Iteration);
        }

    }
}