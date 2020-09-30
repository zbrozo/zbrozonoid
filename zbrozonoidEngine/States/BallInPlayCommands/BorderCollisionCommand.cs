using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class BorderCollisionCommand : ICollisionCommand
    {
        private readonly IEnumerable<IBorder> borders;
        private readonly ICollisionManager collisionManager;
        private BallCollisionState collisionState;

        public BorderCollisionCommand(
            IEnumerable<IBorder> borders, 
            ICollisionManager collisionManager,
            BallCollisionState collisionState)
        {
            this.borders = borders;
            this.collisionManager = collisionManager;
            this.collisionState = collisionState;
        }

        public void Detect(IBall ball)
        { 
            DetectBorderCollision(ball);
        }

        public void Bounce(IBall ball)
        {
            if (!collisionState.CollisionWithBrick &&
                !collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder
                )
            {
                collisionManager.Bounce(collisionState.BordersHitList, ball);
            }
        }

        protected void DetectBorderCollision(IBall ball)
        {
            List<IBorder> bordersHitList = new List<IBorder>();

            foreach (IBorder border in borders)
            {
                var borderCollisionManager = new BorderCollisionManager(collisionManager);
                if (borderCollisionManager.Detect(border, ball))
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
