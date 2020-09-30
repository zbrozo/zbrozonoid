using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.States.BallInPlayCommands;

namespace zbrozonoidEngine.States
{
    public class BallInPlayState : IBallState
    {
        private readonly ICollection<IBrick> bricks;

        private readonly Action<IBall, IEnumerable<int>> handleBrickCollision;
        private readonly Action lostBalls;

        private ILifetimeScope managerScope;

        private static BallCollisionState collisionState = new BallCollisionState();

        private readonly IEnumerable<ICollisionCommand> collisionCommands;

        public BallInPlayState(
            ILifetimeScope scope,
            ICollection<IBrick> bricks,
            Action<IBall, IEnumerable<int>> handleBrickCollision,
            Action lostBalls)
        {
            this.bricks = bricks;
            this.handleBrickCollision = handleBrickCollision;
            this.lostBalls = lostBalls;

            managerScope = scope;

            collisionCommands = new List<ICollisionCommand>() {
                new ScreenCollisionCommand(
                    scope.Resolve<IScreenCollisionManager>(),
                    collisionState),
                new BorderCollisionCommand(
                    scope.Resolve<IBorderManager>(),
                    scope.Resolve<ICollisionManager>(),
                    collisionState),
                new PadCollisionCommand(
                    scope.Resolve<IPadManager>(),
                    scope.Resolve<IBorderManager>(),
                    scope.Resolve<IScreenCollisionManager>(),
                    scope.Resolve<ICollisionManager>(),
                    collisionState),
                new BrickCollisionCommand(
                    bricks,
                    scope.Resolve<ILevelManager>(),
                    scope.Resolve<ITailManager>(),
                    scope.Resolve<ICollisionManager>(),
                    collisionState)
                };
        }

        public bool action(IBall ball)
        {
            ball.MoveBall();
            collisionState.Clear();

            // Phase I - detect collisions
            foreach (var command in collisionCommands)
            {
                command.Detect(ball);
            }

            // Phase II - bounce ball
            foreach (var command in collisionCommands)
            {
                command.Bounce(ball);
            }

            // bounce from border and brick
            if (collisionState.CollisionWithBrick &&
                collisionState.CollisionWithBorder &&
                collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder)
            {
                var hitBricks = bricks.FilterByIndex(collisionState.BricksHitList).Select(x => x.Key).ToArray();
                managerScope.Resolve<ICollisionManager>().Bounce(hitBricks, collisionState.BordersHitList.First(), ball);
            }

            // external actions
            if (collisionState.CollisionWithBrick)
            {
                handleBrickCollision(ball, collisionState.BricksHitList);
            }

            if (collisionState.CollisionWithScreen)
            {
                managerScope.Resolve<IBallManager>().Remove(ball);
                if (managerScope.Resolve<IBallManager>().Count == 0)
                {
                    lostBalls();
                }
            }

            return true;
        }
    }
}
