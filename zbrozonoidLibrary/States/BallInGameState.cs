using System;
using zbrozonoidLibrary.Interfaces;
using zbrozonoidLibrary.Interfaces.States;

namespace zbrozonoidLibrary.States
{
    public class BallInGameState : IBallState
    {
        private Game game;

        public BallInGameState(Game game)
        {
            this.game = game;
        }

        public bool action(IBall ball)
        {
            if (game.ShouldGo)
            {
                ball.MoveBall();
            }

            if (game.HandleScreenCollision(ball))
            {
                ball.SavePosition();
                return false;
            }

            bool borderHit = game.HandleBorderCollision(ball);

            if (game.HandlePadCollision(ball))
            {
                game.SavePosition(ball);
                return false;
            }

            if (game.HandleBrickCollision(ball, borderHit))
            {
                game.SavePosition(ball);
                return false;
            }

            game.SavePosition(ball);
            return true;
        }
    }
}
