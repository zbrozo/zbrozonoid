using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.States.BallInPlayCommands;

namespace zbrozonoidEngine.States
{
    public class BallInPlayState : IBallState
    {
        private IGame game;

        private readonly IHandleCollisionCommand handleScreenCollisionCommand;
        private readonly IHandleCollisionCommand handleBorderCollisionCommand;
        private readonly IHandleCollisionCommand handlePadCollisionCommand;
        private readonly IHandleCollisionCommand handleBrickCollisionCommand;

        private readonly List<IHandleCollisionCommand> collisionCommands;

        private BallCollisionState collisionState = new BallCollisionState();

        public BallInPlayState(IGame game)
        {
            this.game = game;

            handleScreenCollisionCommand = new HandleScreenCollisionCommand(game.ScreenCollisionManager, collisionState);
            handleBorderCollisionCommand = new HandleBorderCollisionCommand(game.BorderManager, game.CollisionManager, collisionState);
            handlePadCollisionCommand = new HandlePadCollisionCommand(game, collisionState);
            handleBrickCollisionCommand = new HandleBrickCollisionCommand(
                                                                            game.LevelManager,
                                                                            game.TailManager,
                                                                            game.CollisionManager,
                                                                            collisionState
                                                                            );

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

            foreach (var command in collisionCommands)
            {
                command.Execute(ball);
            }

            if (collisionState.CollisionWithBrick)
            {
                game.HandleBrickCollision(ball, collisionState.BricksHitList);

                if (collisionState.BounceFromBrick &&
                    !collisionState.BounceFromBorder
                    )
                {
                    game.CollisionManager.Bounce(game.BricksHitList, game.BricksHitList[0], ball);
                }

                if (collisionState.BounceFromBrick &&
                    collisionState.BounceFromBorder
                    )
                {
                    game.CollisionManager.Bounce(game.BricksHitList, collisionState.BordersHitList[0], ball);
                }
            }
            else
            {
                if (collisionState.BounceFromBorder)
                {
                    game.CollisionManager.Bounce(collisionState.BordersHitList, collisionState.BordersHitList[0], ball);
                }

            }

            if (collisionState.BounceFromPad)
            {
                game.CollisionManager.Bounce(collisionState.Pad, ball);
            }

            if (collisionState.CollisionWithScreen)
            {
                game.BallManager.Remove(ball);
                if (game.BallManager.Count == 0)
                {
                    game.LostBalls();
                }
            }

            game.SavePosition(ball);
            return true;
        }

    }
}
