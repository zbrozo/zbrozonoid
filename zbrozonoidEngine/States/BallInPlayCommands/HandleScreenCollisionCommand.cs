using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleScreenCollisionCommand : IHandleCollisionCommand
    {
        private IGame game;

        public bool CollisionResult { set; get; }

        public HandleScreenCollisionCommand(IGame game)
        {
            this.game = game;
        }

        public bool Execute(IBall ball)
        {
            CollisionResult = HandleScreenCollision(ball);

            if (CollisionResult)
            {
                game.BallManager.Remove(ball);

                CheckBallsOutOfScreen();

                return false;
            }

            return true;
        }

        protected bool HandleScreenCollision(IBall ball)
        {
            // in this place you can change to DetectAndVerify to make balls bounce from screen borders
            if (game.ScreenCollisionManager.Detect(ball))
            {
                return true;
            }
            return false;
        }

        protected void CheckBallsOutOfScreen()
        {
            if (game.BallManager.Count == 0)
            {
                game.LostBalls();
            }
        }

    }
}
