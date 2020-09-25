using System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class BallFactory
    {
        private readonly IBallManager ballManager;
        private readonly ITailManager tailManager;
        private readonly IPadManager padManager;
        private readonly IGameConfig gameConfig;
        private readonly Action<IBall, int> SpeedTimerHandler;

        public BallFactory(
            IBallManager ballManager,
            ITailManager tailManager,
            IPadManager padManager,
            IGameConfig gameConfig,
            Action<IBall, int> SpeedTimerHandler)
        {
            this.ballManager = ballManager;
            this.tailManager = tailManager;
            this.padManager = padManager;
            this.gameConfig = gameConfig;
            this.SpeedTimerHandler = SpeedTimerHandler;
        }

        public void CreateBalls()
        {
            ballManager.Clear();
            tailManager.Clear();

            var padIterator = padManager.GetEnumerator();
            for (int i = 0; i < gameConfig.Players && i < gameConfig.Mouses; ++i)
            {
                padIterator.MoveNext();

                IPad pad = padIterator.Current.Item3;
                CreateBall(pad);
            }
        }

        public void CreateBall(IPad pad)
        {
            IBall ball = CreateBall();
            padManager.SetBallStartPosition(pad, ball);
            ballManager.Add(ball, pad);
        }

        private IBall CreateBall()
        {
            const int defaultDegree = 45;
            var defaultDirection = new Vector2(1, 1);
            var defaultOffset = new Vector2(0, 0);
            IBall ball = new Ball(new RandomGenerator(),
                                  new LinearMovement(0, defaultDegree, defaultOffset, defaultDirection)
                            )
            {
                BallSpeedTimerCallback = SpeedTimerHandler
            };

            ball.SetSize(15, 15);
            return ball;
        }

    }
}
