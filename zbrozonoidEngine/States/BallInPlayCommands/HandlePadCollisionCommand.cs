using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandlePadCollisionCommand : IHandleCollisionCommand
    {
        private IGame game;

        private readonly ICollisionManager collisionManagerForMoveReversion = new CollisionManager();
        private BallCollisionState collisionState;

        private const int padSpeedToGoBallFaster = 5;

        public HandlePadCollisionCommand(IGame game, BallCollisionState collisionState)
        {
            this.game = game;
            this.collisionState = collisionState;
        }

        public void Execute(IBall ball)
        {
            HandlePadCollision(ball);
        }

        protected void HandlePadCollision(IBall ball)
        {
            IPad pad = null;

            bool collisionDetected = false;
            foreach (var value in game.PadManager)
            {
                pad = value.Item3;
                if (game.CollisionManager.Detect(pad, ball))
                {
                    collisionDetected = true;

                    CorrectBallPosition(pad, ball);

                    if (game.PadCurrentSpeed > padSpeedToGoBallFaster)
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
                    game.PadManager.RestartBallYPosition(pad, ball);
                    return;
                }

                previous = ball.Boundary.Min;
                ball.SavePosition();

                foreach (IBorder border in game.BorderManager)
                {
                    if (collisionManagerForMoveReversion.Detect(border, ball))
                    {
                        game.PadManager.SetBallStartPosition(pad, ball);
                        break;
                    }
                }

                if (game.ScreenCollisionManager.DetectAndVerify(ball))
                {
                    game.PadManager.SetBallStartPosition(pad, ball);
                    break;
                }

            }

            ball.Boundary.Min = previous;

        }

    }
}
