using System;
using System.Collections.Generic;
using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class HandleBrickCollisionCommand : IHandleCollisionCommand
    {
        private readonly Game game;
        private readonly IHandleCollisionCommand borderCollisionCommand;

        public bool CollisionResult { set; get; }

        public HandleBrickCollisionCommand(Game game, IHandleCollisionCommand borderCollisionCommand)
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
                game.HandleBrickCollision(bricksHit);

                bool destroyerBall = game.IsBallDestroyer(ball);
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
                if (brick.Hit || !brick.IsVisible())
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
