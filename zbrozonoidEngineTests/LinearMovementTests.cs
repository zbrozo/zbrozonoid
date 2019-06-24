using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngineTests
{
    public class LinearMovementTests
    {
        private const int Iterations = 1000;

        [SetUp]
        public void Setup()
        {
        }

        [TestCase(1, 1, 965, 258)]
        [TestCase(1, -1, 965, -258)]
        [TestCase(-1, 1, -965, 258)]
        [TestCase(-1, -1, -965, -258)]
        public void VerifyIterationsFor15Degrees(int directionX, int directionY, int expectedX, int expectedY)
        {
            // Given
            const int Degree = 15;
            IMovement movement = new LinearMovement(0, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = Move(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
        }


        [TestCase(1, 1, 707, 707)]
        [TestCase(1, -1, 707, -707)]
        [TestCase(-1, 1, -707, 707)]
        [TestCase(-1, -1, -707, -707)]
        public void VerifyIterationsFor45Degrees(int directionX, int directionY, int expectedX, int expectedY)
        {
            // Given
            const int Degree = 45;
            IMovement movement = new LinearMovement(0, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = Move(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
        }

        [TestCase(1, 1, 342, 939)]
        [TestCase(1, -1, 342, -939)]
        [TestCase(-1, 1, -342, 939)]
        [TestCase(-1, -1, -342, -939)]
        public void VerifyIterationsFor70Degrees(int directionX, int directionY, int expectedX, int expectedY)
        {
            // Given
            const int Degree = 70;
            IMovement movement = new LinearMovement(0, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = Move(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
        }

        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void VerifyReverseIterationsFor15Degrees(int directionX, int directionY)
        {
            // Given
            const int Degree = 15;
            IMovement movement = new LinearMovement(Iterations, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = ReverseMove(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
        }

        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void VerifyReverseIterationsFor45Degrees(int directionX, int directionY)
        {
            // Given
            const int Degree = 45;
            IMovement movement = new LinearMovement(Iterations, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = ReverseMove(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
        }

        [TestCase(1, 1)]
        [TestCase(1, -1)]
        [TestCase(-1, 1)]
        [TestCase(-1, -1)]
        public void VerifyReverseMoveFor70Degrees(int directionX, int directionY)
        {
            // Given
            const int Degree = 70;
            IMovement movement = new LinearMovement(Iterations, Degree, new Vector2(0, 0), new Vector2(directionX, directionY));

            // Then
            bool result = ReverseMove(movement, Iterations, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
        }
       
        private bool Move(IMovement move, int count, out Vector2 position)
        {
            position = new Vector2();
            for (int i = 0; i < count; i++)
            {
                bool result = move.Move(out position);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }

        private bool ReverseMove(IMovement move, int count, out Vector2 position)
        {
            position = new Vector2();
            for (int i = 0; i < count; i++)
            {
                bool result = move.ReverseMove(out position);
                if (!result)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
