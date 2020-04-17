using System.Collections.Generic;
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

            bool result = DetectBrickCollision(ball, out List<BrickHit> bricksHitList);
            if (result)
            { 
                if (isDestroyer)
                {
                    foreach (var brick in bricksHitList)
                    {
                        if (!brick.Brick.IsBeatable)
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

        private bool DetectBrickCollision(IBall ball, out List<BrickHit> bricksHitList)
        {
            List<BrickHit> bricks = new List<BrickHit>();

            bool result = false;
            int id = 0;
            foreach (var value in levelManager.GetCurrent().Bricks)
            {
                IBrick brick = value;
                if (brick.IsHit || !brick.IsVisible)
                {
                    ++id;
                    continue;
                }

                if (collisionManager.Detect(brick, ball))
                {
                    bricks.Add(new BrickHit(id, brick));
                    result = true;
                }
                ++id;
            }

            bricksHitList = bricks;
            return result;
        }

    }
}
