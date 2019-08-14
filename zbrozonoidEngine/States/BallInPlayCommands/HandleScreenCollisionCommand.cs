using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class HandleScreenCollisionCommand : IBallInPlayCommand
    {
        private Game game;

        public HandleScreenCollisionCommand(Game game)
        {
            this.game = game;
        }

        public bool Execute(IBall ball)
        {
            if (HandleScreenCollision(ball))
            {
                game.GameState.BallsOutOfScreen++;

                CheckBallsOutOfScreen();

                ball.SavePosition();
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
