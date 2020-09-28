using zbrozonoidEngine.Counters;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class BallExtraFeatureBuilder
    {
        private IBallManager ballManager;
        private ITailManager tailManager;
        private BallBuilder ballBuilder;
        private FireBallCounter fireBallCounter;

        public BallExtraFeatureBuilder(
            IBallManager ballManager, 
            ITailManager tailManager, 
            BallBuilder ballBuilder,
            FireBallCounter fireBallCounter)
        {
            this.ballManager = ballManager;
            this.tailManager = tailManager;
            this.ballBuilder = ballBuilder;
            this.fireBallCounter = fireBallCounter;
        }

        public void Create(IBall ball, BrickType type)
        {
            switch (type)
            {
                case BrickType.ThreeBalls:
                    {
                        IPad pad = ballManager.GetPadAssignedToBall(ball);
                        ballBuilder.Create(pad);
                        ballBuilder.Create(pad);
                        break;
                    }
                case BrickType.DestroyerBall:
                    {
                        ITail tail = new Tail { FireBallTimerCallback = fireBallCounter.FireBallTimerHandler };
                        tailManager.Add(ball, tail);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}
