using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class HandleBorderCollisionCommand : IBallInPlayCommand
    {
        private Game game;

        public bool CollisionResult { set; get; }

        public HandleBorderCollisionCommand(Game game)
        {
            this.game = game;
        }

        public bool Execute(IBall ball)
        { 
            CollisionResult = HandleBorderCollision(ball);
            return true;
        }

        protected bool HandleBorderCollision(IBall ball)
        {
            foreach (IBorder border in game.BorderManager)
            {
                IBorderCollisionManager borderCollisionManager = new BorderCollisionManager(border, game.CollisionManager);
                if (borderCollisionManager.DetectAndVerify(game.BricksHitList, ball))
                {
                    return true;
                }
            }
            return false;
        }

    }
}
