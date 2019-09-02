using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandlePadCollisionCommand : IHandleCollisionCommand
    {
        private IGame game;

        public bool CollisionResult { set; get; }

        private readonly ICollisionManager collisionManagerForMoveReversion;

        private const int padSpeedToGoBallFaster = 10;

        public HandlePadCollisionCommand(IGame game)
        {
            this.game = game;
            collisionManagerForMoveReversion = new CollisionManager();
        }

        public bool Execute(IBall ball)
        {
            CollisionResult = HandlePadCollision(ball);
            return !CollisionResult;
        }

        protected bool HandlePadCollision(IBall ball)
        {
            foreach (IPad pad in game.PadManager)
            {
                if (game.CollisionManager.Detect(pad, ball))
                {
                    pad.LogData();

                    CorrectBallPosition(pad, ball);
                    game.CollisionManager.Bounce(pad, ball);

                    if (game.PadCurrentSpeed > padSpeedToGoBallFaster)
                    {
                        ball.GoFaster();
                    }

                    ball.LogData();
                    return true;
                }
            }
            return false;
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
