namespace UnitTestProject1
{
    using NUnit.Framework;
    using Moq;

    using zbrozonoidLibrary;
    using zbrozonoidLibrary.Interfaces;

    using zbrozonoid.CollisionManagers;

    [TestFixture]
    public class UnitTest1
    {
        IScreen screen = new Screen();
        Mock<ILoggerBase> logger;
        Mock<IRandomGenerator> randomGenerator;

        [SetUp]
        public void SetUp()
        {
            logger = new Mock<ILoggerBase>();
            Logger.Instance = logger.Object;

            randomGenerator = new Mock<IRandomGenerator>();

            screen.Width = 1024;
            screen.Height = 768;
        }

        [Test]
        public void VerifyCollisionMoveReverseAndBounce()
        {
            // arrange
            IBall ball = new Ball(randomGenerator.Object);
            IElement ballPosition = ball as IElement;

            ballPosition.PosX = 168;
            ballPosition.PosY = 40;
            ballPosition.Width = 15;
            ballPosition.Height = 15;
            ball.OffsetX = 422;
            ball.OffsetY = 285;
            ball.DirectionX = -1;
            ball.DirectionY = -1;
            ball.Iteration = 354;
            ball.Degree = 44;

            IPad pad = new Pad();
            IElement padPosition = pad as IElement;
            padPosition.PosX = 69;
            padPosition.PosY = 20;
            padPosition.Width = 100;
            padPosition.Height = 24;

            ICollisionManager collisionManager = new CollisionManager();

            // act
            while (collisionManager.Detect(pad, ball))
            {
                ball.MoveBall(true);
                ball.SavePosition();
            }

            collisionManager.Bounce(ball);

            // assert
            Assert.AreEqual(169, ballPosition.PosX);
            Assert.AreEqual(40, ballPosition.PosY);
            Assert.AreEqual(169, ball.OffsetX);
            Assert.AreEqual(40, ball.OffsetY);
            Assert.AreEqual(1, ball.DirectionX);
            Assert.AreEqual(1, ball.DirectionY);
            Assert.AreEqual(0, ball.Iteration);
        }

        [Test]
        public void VerifyCollisionMoveReverseIsFalse()
        {
            // arrange
            IBall ball = new Ball(randomGenerator.Object);
            IElement ballPosition = ball as IElement;

            ballPosition.PosX = 169;
            ballPosition.PosY = 40;
            ballPosition.Width = 15;
            ballPosition.Height = 15;
            ball.OffsetX = 169;
            ball.OffsetY = 40;
            ball.DirectionX = 1;
            ball.DirectionY = 1;
            ball.Iteration = 1;
            ball.Degree = 29;

            IPad pad = new Pad();
            IElement padPosition = pad as IElement;
            padPosition.PosX = 108;
            padPosition.PosY = 20;
            padPosition.Width = 100;
            padPosition.Height = 24;

            ICollisionManager collisionManager = new CollisionManager();

            // act
            while (collisionManager.Detect(pad, ball))
            {
                bool result = ball.MoveBall(true);
                if (!result)
                {
                    break;
                }
                ball.SavePosition();
            }

            // assert
            Assert.AreEqual(169, ballPosition.PosX);
            Assert.AreEqual(40, ballPosition.PosY);
            Assert.AreEqual(169, ball.OffsetX);
            Assert.AreEqual(40, ball.OffsetY);
            Assert.AreEqual(1, ball.DirectionX);
            Assert.AreEqual(1, ball.DirectionY);
            Assert.AreEqual(-1, ball.Iteration);
        }
    }
}
