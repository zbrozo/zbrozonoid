using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class BrickCollisionCommand : ICollisionCommand
    {
        private readonly ICollection<IBrick> bricks;
        private readonly ILevelManager levelManager;
        private readonly ICollisionManager collisionManager;
        private readonly ITailManager tailManager;
        private BallCollisionState collisionState;

        public BrickCollisionCommand(
            ICollection<IBrick> bricks,
            ILevelManager levelManager,
            ITailManager tailManager, 
            ICollisionManager collisionManager,
            BallCollisionState collisionState)
        {
            this.bricks = bricks;
            this.levelManager = levelManager;
            this.collisionManager = collisionManager;
            this.tailManager = tailManager;
            this.collisionState = collisionState;
        }

        public void Detect(IBall ball)
        {
            DetectBrickCollision(ball, tailManager?.Find(ball) != null);
        }

        public void Bounce(IBall ball)
        {
            if (collisionState.CollisionWithBrick &&
                collisionState.BounceFromBrick &&
                !collisionState.BounceFromBorder
                )
            {
                var hitBricks = bricks.FilterByIndex(collisionState.BricksHitList).Select(x => x.Key).ToArray();
                collisionManager.Bounce(hitBricks, ball);
            }
        }

        private void DetectBrickCollision(IBall ball, bool isDestroyer)
        {
            bool bounce = false;

            bool result = DetectBrickCollision(ball, out List<int> bricksHitList);
            if (result)
            { 
                if (isDestroyer)
                {
                    foreach (var number in bricksHitList)
                    {
                        if (!bricks.ElementAt(number).IsBeatable)
                        {
                            bounce = true;
                        }
                    }
                }
                else
                {
                    bounce = true;
                }

                collisionState.SetBrickCollisionState(true, bounce, bricksHitList); 
            }
        }

        private bool DetectBrickCollision(IBall ball, out List<int> bricksHitList)
        {
            bricksHitList = bricks.DetectCollision(ball, collisionManager).ToList();
            return bricksHitList.Any();
        }
    }
}
