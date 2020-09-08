using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleBrickCollisionCommand : IHandleCollisionCommand
    {
        private readonly List<IBrick> bricks;
        private readonly ILevelManager levelManager;
        private readonly ICollisionManager collisionManager;
        private readonly ITailManager tailManager;
        private BallCollisionState collisionState;

        public HandleBrickCollisionCommand(List<IBrick> bricks,
                                           ILevelManager levelManager,
                                           ITailManager tailManager, 
                                           ICollisionManager collisionManager,
                                           BallCollisionState collisionState
                                           )
        {
            this.bricks = bricks;
            this.levelManager = levelManager;
            this.collisionManager = collisionManager;
            this.tailManager = tailManager;
            this.collisionState = collisionState;
        }

        public void Execute(IBall ball)
        {
            HandleBrickCollision(ball, tailManager?.Find(ball) != null);
        }

        private void HandleBrickCollision(IBall ball, bool isDestroyer)
        {
            bool bounce = false;

            bool result = DetectBrickCollision(ball, out List<int> bricksHitList);
            if (result)
            { 
                if (isDestroyer)
                {
                    foreach (var number in bricksHitList)
                    {
                        if (!bricks[number].IsBeatable)
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
