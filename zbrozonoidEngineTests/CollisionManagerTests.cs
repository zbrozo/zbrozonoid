using Moq;
using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

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
            Assert.IsTrue(manager.Flags.OverlapInsideFull());
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(14, true)]
        [TestCase(15, false)]
        public void VerifyCollisionInsideLeft(int posX, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(posX, 0);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapInsideLeft());
        }

        [TestCase(0, false)]
        [TestCase(-1, true)]
        [TestCase(-14, true)]
        [TestCase(-15, false)]
        public void VerifyCollisionInsideRight(int posX, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(posX, 0);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapInsideRight());
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(14, true)]
        [TestCase(15, false)]
        public void VerifyCollisionInsideTop(int posY, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(0, posY);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapInsideTop());
        }

        [TestCase(0, false)]
        [TestCase(-1, true)]
        [TestCase(-14, true)]
        [TestCase(-15, false)]
        public void VerifyCollisionInsideBottom(int posY, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(0, posY);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapInsideBottom());
        }


        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(14, true)]
        [TestCase(15, false)]
        public void VerifyCollisionOutsideLeft(int posX, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.SetSize(15, 25);
            ball2.Boundary.Min = new Vector2(posX, -5);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapOutsideLeft());
        }

        [TestCase(0, false)]
        [TestCase(-1, true)]
        [TestCase(-14, true)]
        [TestCase(-15, false)]
        public void VerifyCollisionOutsideRight(int posX, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.SetSize(15, 25);
            ball2.Boundary.Min = new Vector2(posX, -5);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapOutsideRight());
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(14, true)]
        [TestCase(15, false)]
        public void VerifyCollisionOutsideTop(int posY, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.SetSize(25, 15);
            ball2.Boundary.Min = new Vector2(-5, posY);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapOutsideTop());
        }

        [TestCase(0, false)]
        [TestCase(-1, true)]
        [TestCase(-14, true)]
        [TestCase(-15, false)]
        public void VerifyCollisionOutsideBottom(int posY, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.SetSize(25, 15);
            ball2.Boundary.Min = new Vector2(-5, posY);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapOutsideBottom());
        }

        [TestCase(1, 1, true)]
        [TestCase(14, 14, true)]
        [TestCase(15, 15, false)]
        public void VerifyCollisionCornerTopLeft(int x, int y, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(x, y);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapCornerTopLeft());
        }

        [TestCase(-1, 1, true)]
        [TestCase(-14, 14, true)]
        [TestCase(-15, 15, false)]
        public void VerifyCollisionCornerTopRight(int x, int y, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(x, y);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapCornerTopRight());
        }

        [TestCase(1, -1, true)]
        [TestCase(14, -14, true)]
        [TestCase(15, -15, false)]
        public void VerifyCollisionCornerBottomLeft(int x, int y, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(x, y);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapCornerBottomLeft());
        }

        [TestCase(-1, -1, true)]
        [TestCase(-14, -14, true)]
        [TestCase(-15, -15, false)]
        public void VerifyCollisionCornerBottomRight(int x, int y, bool result)
        {
            // Given
            ICollisionManager manager = new CollisionManager();
            ball2.Boundary.Min = new Vector2(x, y);

            // When
            manager.Detect(ball1, ball2);

            // Then
            Assert.AreEqual(result, manager.Flags.OverlapCornerBottomRight());
        }

    }
}