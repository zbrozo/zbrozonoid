using System;
using System.Collections.Generic;
using System.Linq;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleBrickCollisionCommand : IHandleCollisionCommand
    {
        private readonly ILevelManager levelManager;
        private readonly ICollisionManager collisionManager;
        private readonly ITailManager tailManager;
        private BallCollisionState collisionState;

        public HandleBrickCollisionCommand(ILevelManager levelManager,
                                           ITailManager tailManager, 
                                           ICollisionManager collisionManager,
                                           BallCollisionState collisionState
                                           )
        {
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

            bool result = DetectBrickCollision(ball, out List<BrickWithNumber> bricksHitList);
            if (result)
            { 
                if (isDestroyer)
                {
                    foreach (var brick in bricksHitList)
                    {
                        if (!brick.IsBeatable)
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

        private bool DetectBrickCollision(IBall ball, out List<BrickWithNumber> bricksHitList)
        {
            var count = levelManager.GetCurrent().Bricks.Count();
            IEnumerable<BrickWithNumber> bricksWithNumbers = Enumerable.Range(1,count).Zip(levelManager.GetCurrent().Bricks, (id, brick) => new BrickWithNumber(id, brick));
            bricksHitList = bricksWithNumbers.DetectCollision(ball, collisionManager).ToList();
            return bricksHitList.Any();
        }
    }
}
