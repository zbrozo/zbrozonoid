using System;
using System.Collections.Generic;
using Autofac;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.States.BallInPlayCommands;

namespace zbrozonoidEngine.States
{
    public class BallInPlayState : IBallState
    {
        private ICollection<IBrick> bricks;

        private Action<IBall> savePosition;
        private Action<IBall, List<int>> handleBrickCollision;
        private Action lostBalls;

        private ILifetimeScope managerScope;

        private static BallCollisionState collisionState = new BallCollisionState();

        private readonly IHandleCollisionCommand handleScreenCollisionCommand;
        private readonly IHandleCollisionCommand handleBorderCollisionCommand;
        private readonly IHandleCollisionCommand handlePadCollisionCommand;
        private readonly IHandleCollisionCommand handleBrickCollisionCommand;

        private readonly List<IHandleCollisionCommand> collisionCommands;

        public BallInPlayState(
            ILifetimeScope scope, 
            ICollection<IBrick> bricks, 
            Action<IBall> savePosition,
            Action<IBall, List<int>> handleBrickCollision,
            Action lostBalls)
        {
            this.bricks = bricks;
            this.savePosition = savePosition;
            this.handleBrickCollision = handleBrickCollision;
            this.lostBalls = lostBalls;

            managerScope = scope;

            handleScreenCollisionCommand = new HandleScreenCollisionCommand(
                scope.Resolve<IScreenCollisionManager>(), 
                collisionState);
            handleBorderCollisionCommand = new HandleBorderCollisionCommand(
                scope.Resolve<IBorderManager>(), 
                scope.Resolve<ICollisionManager>(), 
                collisionState);
            handlePadCollisionCommand = new HandlePadCollisionCommand(
                scope.Resolve<IPadManager>(),
                scope.Resolve<IBorderManager>(),
                scope.Resolve<IScreenCollisionManager>(),
                scope.Resolve<ICollisionManager>(), 
                collisionState);
            handleBrickCollisionCommand = new HandleBrickCollisionCommand(  
                bricks,
                scope.Resolve<ILevelManager>(),
                scope.Resolve<ITailManager>(),
                scope.Resolve<ICollisionManager>(),
                collisionState);

            collisionCommands = new List<IHandleCollisionCommand>() { 
                handleScreenCollisionCommand,
                handleBorderCollisionCommand,
                handlePadCollisionCommand,
                handleBrickCollisionCommand
                };
        }

        public bool action(IBall ball)
        {
            ball.MoveBall();
            collisionState.Clear();

            // Phase I
            foreach (var command in collisionCommands)
            {
                command.Execute(ball);
            }

            // Phase II
            BrickCollisionHandler(ball);
            BrickBounceHandler(ball);
            BricksAndBorderBounceHandler(ball);
            BorderBounceHandler(ball);
            PadBounceHandler(ball);
            ScreenCollisionHandler(ball);

            savePosition(ball);
            return true;
        }

        private void BrickCollisionHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick)
            {
                handleBrickCollision(ball, collisionState.BricksHitList);
            }
        }

        private void BrickBounceHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick &&
                collisionState.BounceFromBrick &&
                !collisionState.BounceFromBorder
                )
            {
                var bricks = this.bricks.FilterByIndex(collisionState.BricksHitList);
                managerScope.Resolve<ICollisionManager>().Bounce(bricks, bricks[0], ball);
            }
        }

        private void BricksAndBorderBounceHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick &&
                collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder
                )
            {
                var bricks = this.bricks.FilterByIndex(collisionState.BricksHitList);
                managerScope.Resolve<ICollisionManager>().Bounce(bricks, bricks[0], ball);
            }
        }

        private void BorderBounceHandler(IBall ball)
        {
            if (!collisionState.CollisionWithBrick &&
                !collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder
                )
            {
                managerScope.Resolve<ICollisionManager>().Bounce(collisionState.BordersHitList, collisionState.BordersHitList[0], ball);
            }
        }

        private void PadBounceHandler(IBall ball)
        {
            if (collisionState.BounceFromPad)
            {
                managerScope.Resolve<ICollisionManager>().Bounce(collisionState.Pad, ball);
            }
        }

        private void ScreenCollisionHandler(IBall ball)
        {
            if (collisionState.CollisionWithScreen)
            {
                managerScope.Resolve<IBallManager>().Remove(ball);
                if (managerScope.Resolve<IBallManager>().Count == 0)
                {
                    lostBalls();
                }
            }
        }
    }
}
