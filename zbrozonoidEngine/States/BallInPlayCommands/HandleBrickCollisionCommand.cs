using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine.States.BallInPlayCommands
{
    public class HandleBrickCollisionCommand : IHandleCollisionCommand
    {
        private readonly IGame game;
        private readonly IHandleCollisionCommand borderCollisionCommand;

        public bool CollisionResult { set; get; }

        public HandleBrickCollisionCommand(IGame game, IHandleCollisionCommand borderCollisionCommand)
        {
            this.game = game;
            this.borderCollisionCommand = borderCollisionCommand;
        }

        public bool Execute(IBall ball)
        {
            CollisionResult = HandleBrickCollision(ball, borderCollisionCommand.CollisionResult);
            return !CollisionResult;
        }

        protected bool HandleBrickCollision(IBall ball, bool borderHit)
        {
            game.BricksHitList.Clear();

            bool result = DetectBrickCollision(ball, out List<BrickHit> bricksHit);
            if (result)
            {
                game.HandleBrickCollision(ball, bricksHit);

                bool destroyerBall = game.IsBallDestroyer(ball);

                if (destroyerBall)
                {
                    foreach (var brick in bricksHit)
                    {
                        if (!brick.Brick.IsBeatable)
                        {
                            game.CollisionManager.Bounce(game.BricksHitList, game.BricksHitList[0], ball);
                        }
                    }
                }

                if (!borderHit && !destroyerBall)
                {
                    game.CollisionManager.Bounce(game.BricksHitList, game.BricksHitList[0], ball);
                }

                return true;
            }

            return false;
        }

        private bool DetectBrickCollision(IBall ball, out List<BrickHit> bricksHit)
        {
            bricksHit = new List<BrickHit>();
            List<IBrick> bricks = game.LevelManager.GetCurrent().Bricks;

            bool result = false;
            int id = 0;
            foreach (var value in bricks)
            {
                IBrick brick = value;
                if (brick.IsHit || !brick.IsVisible)
                {
                    ++id;
                    continue;
                }

                if (game.CollisionManager.Detect(brick, ball))
                {
                    bricksHit.Add(new BrickHit(id, brick));
                    result = true;
                }
                ++id;
            }
            return result;
        }

    }
}
