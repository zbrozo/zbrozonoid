using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleBorderCollisionCommand : IHandleCollisionCommand
    {
        private IGame game;

        public bool CollisionResult { set; get; }

        public HandleBorderCollisionCommand(IGame game)
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
