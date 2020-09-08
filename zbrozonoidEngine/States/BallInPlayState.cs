using System;
using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.States.BallInPlayCommands;

namespace zbrozonoidEngine.States
{
    public class BallInPlayState : IBallState
    {
        private static IGame gameMain;
        private static BallCollisionState collisionState = new BallCollisionState();

        private readonly IHandleCollisionCommand handleScreenCollisionCommand;
        private readonly IHandleCollisionCommand handleBorderCollisionCommand;
        private readonly IHandleCollisionCommand handlePadCollisionCommand;
        private readonly IHandleCollisionCommand handleBrickCollisionCommand;

        private readonly List<IHandleCollisionCommand> collisionCommands;

        public BallInPlayState(IGame game)
        {
            gameMain = game;

            handleScreenCollisionCommand = new HandleScreenCollisionCommand(game.ScreenCollisionManager, collisionState);
            handleBorderCollisionCommand = new HandleBorderCollisionCommand(game.BorderManager, game.CollisionManager, collisionState);
            handlePadCollisionCommand = new HandlePadCollisionCommand(game, collisionState);
            handleBrickCollisionCommand = new HandleBrickCollisionCommand(  game.BricksWithNumbers,
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

            gameMain.SavePosition(ball);
            return true;
        }

        private void BrickCollisionHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick)
            {
                gameMain.HandleBrickCollision(ball, collisionState.BricksHitList);
            }
        }

        private void BrickBounceHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick &&
                collisionState.BounceFromBrick &&
                !collisionState.BounceFromBorder
                )
            {
                gameMain.CollisionManager.Bounce(collisionState.BricksHitList.ToBricks(), collisionState.BricksHitList[0], ball);
            }
        }

        private void BricksAndBorderBounceHandler(IBall ball)
        {
            if (collisionState.CollisionWithBrick &&
                collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder
                )
            {
                gameMain.CollisionManager.Bounce(collisionState.BricksHitList.ToBricks(), collisionState.BordersHitList[0], ball);
            }
        }

        private void BorderBounceHandler(IBall ball)
        {
            if (!collisionState.CollisionWithBrick &&
                !collisionState.BounceFromBrick &&
                collisionState.BounceFromBorder
                )
            {
                gameMain.CollisionManager.Bounce(collisionState.BordersHitList, collisionState.BordersHitList[0], ball);
            }
        }

        private void PadBounceHandler(IBall ball)
        {
            if (collisionState.BounceFromPad)
            {
                gameMain.CollisionManager.Bounce(collisionState.Pad, ball);
            }
        }

        private void ScreenCollisionHandler(IBall ball)
        {
            if (collisionState.CollisionWithScreen)
            {
                gameMain.BallManager.Remove(ball);
                if (gameMain.BallManager.Count == 0)
                {
                    gameMain.LostBalls();
                }
            }
        }
    }
}
