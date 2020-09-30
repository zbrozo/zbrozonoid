using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class PadCollisionCommand : ICollisionCommand
    {
        private readonly ICollisionManager collisionManagerForMoveReversion = new CollisionManager();

        private IPadManager padManager;
        private ICollisionManager collisionManager;
        private IScreenCollisionManager screenCollisionManager;
        private IBorderManager borderManager;
        private BallCollisionState collisionState;

        private const int padSpeedToGoBallFaster = 5;

        public PadCollisionCommand(
            IPadManager padManager, 
            IBorderManager borderManager,
            IScreenCollisionManager screenCollisionManager,
            ICollisionManager collisionManager, 
            BallCollisionState collisionState)
        {
            this.padManager = padManager;
            this.collisionManager = collisionManager;
            this.screenCollisionManager = screenCollisionManager;
            this.borderManager = borderManager;
            this.collisionState = collisionState;
        }

        public void Detect(IBall ball)
        {
            DetectPadCollision(ball);
        }

        public void Bounce(IBall ball)
        {
            if (collisionState.BounceFromPad)
            {
                collisionManager.Bounce(collisionState.Pad, ball);
            }
        }

        protected void DetectPadCollision(IBall ball)
        {
            IPad pad = null;

            bool collisionDetected = false;
            foreach (var value in padManager)
            {
                pad = value.Item3;
                if (collisionManager.Detect(pad, ball))
                {
                    collisionDetected = true;

                    CorrectBallPosition(pad, ball);

                    if (pad.Speed > padSpeedToGoBallFaster)
                    {
                        ball.SetFasterSpeed();
                    }

                    break;
                }
            }

            if (collisionDetected)
            {
                collisionState.SetPadCollistionState(true, true, pad);
            }
        }

        private void CorrectBallPosition(IPad pad, IBall ball)
        {
            Vector2 previous = ball.Boundary.Min;
            while (collisionManagerForMoveReversion.Detect(pad, ball))
            {
                if (!ball.MoveBall(true))
                {
                    ball.Boundary.Min = previous;
                    padManager.RestartBallYPosition(pad, ball);
                    return;
                }

                previous = ball.Boundary.Min;
                ball.SavePosition();

                foreach (IBorder border in borderManager)
                {
                    if (collisionManagerForMoveReversion.Detect(border, ball))
                    {
                        padManager.SetBallStartPosition(pad, ball);
                        break;
                    }
                }

                if (screenCollisionManager.DetectAndVerify(ball))
                {
                    padManager.SetBallStartPosition(pad, ball);
                    break;
                }

            }

            ball.Boundary.Min = previous;

        }

    }
}
