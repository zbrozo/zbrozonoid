using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleBorderCollisionCommand : IHandleCollisionCommand
    {
        private readonly IBorderManager borderManager;
        private readonly ICollisionManager collisionManager;
        private BallCollisionState collisionState;

        public HandleBorderCollisionCommand(IBorderManager borderManager, 
                                            ICollisionManager collisionManager,
                                            BallCollisionState collisionState)
        {
            this.borderManager = borderManager;
            this.collisionManager = collisionManager;
            this.collisionState = collisionState;
        }

        public void Execute(IBall ball)
        { 
            HandleBorderCollision(ball);
        }

        protected void HandleBorderCollision(IBall ball)
        {
            List<IBorder> bordersHitList = new List<IBorder>();

            foreach (IBorder border in borderManager)
            {
                IBorderCollisionManager borderCollisionManager = new BorderCollisionManager(border, collisionManager);
                if (borderCollisionManager.Detect(ball))
                {
                    bordersHitList.Add(border);
                }
            }

            if (bordersHitList.Count > 0)
            {
                collisionState.SetBorderCollistionState(true, true, bordersHitList);
            }
        }

    }
}
