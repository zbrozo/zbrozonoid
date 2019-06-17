using NUnit.Framework;
using zbrozonoidLibrary;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoidEngineTests
{
    public class LinearMovementTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void VerifyFirstMove()
        {
            // Given
            IMovement move = new LinearMovement();

            // Then
            bool result = move.Move();

            // When
            Assert.IsTrue(result);
        }
    }
}
