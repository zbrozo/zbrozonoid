using NUnit.Framework;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngineTests
{
    public class LinearMovementTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VerifyMoveForFirstStep()
        {
            // Given
            IMovement movement = new LinearMovement();

            movement.Direction = new Vector2(1, 1);
            movement.Offset = new Vector2(0, 0);
            movement.Degree = 45;
            movement.Iteration = 0;

            // Then
            movement.Move(out Vector2 position);

            // When
            Assert.AreEqual(0, position.X);
            Assert.AreEqual(0, position.Y);
        }

        [TestCase(10, 9, 2)]
        [TestCase(20, 19, 5)]
        public void VerifyMoveFor15Degrees(int count, int expectedX, int expectedY)
        {
            // Given
            IMovement movement = new LinearMovement();

            movement.Direction = new Vector2(1, 1);
            movement.Offset = new Vector2(0, 0);
            movement.Degree = 15;
            movement.Iteration = 0;

            // Then
            bool result = Move(movement, count, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
        }

        [TestCase(10, 7, 7)]
        [TestCase(20, 14, 14)]
        public void VerifyMoveFor45Degrees(int count, int expectedX, int expectedY)
        {
            // Given
            IMovement movement = new LinearMovement();

            movement.Direction = new Vector2(1, 1);
            movement.Offset = new Vector2(0, 0);
            movement.Degree = 45;
            movement.Iteration = 0;

            // Then
            bool result = Move(movement, count, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
        }

        [TestCase(10, 3, 9)]
        [TestCase(20, 6, 18)]
        public void VerifyMoveFor70Degrees(int count, int expectedX, int expectedY)
        {
            // Given
            IMovement movement = new LinearMovement();

            movement.Direction = new Vector2(1, 1);
            movement.Offset = new Vector2(0, 0);
            movement.Degree = 70;
            movement.Iteration = 0;

            // Then
            bool result = Move(movement, count, out Vector2 position);

            // When
            Assert.IsTrue(result);
            Assert.AreEqual(expectedX, position.X);
            Assert.AreEqual(expectedY, position.Y);
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

    }
}
