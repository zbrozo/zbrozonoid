using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleScreenCollisionCommand : IHandleCollisionCommand
    {
        private Game game;

        public bool CollisionResult { set; get; }

        public HandleScreenCollisionCommand(Game game)
        {
            this.game = game;
        }

        public bool Execute(IBall ball)
        {
            CollisionResult = HandleScreenCollision(ball);

            if (CollisionResult)
            {
                game.GameState.BallsOutOfScreen++;

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
            if (game.GameState.BallsOutOfScreen == game.BallManager.Count)
            {
                game.LostBalls();
            }
        }

    }
}
