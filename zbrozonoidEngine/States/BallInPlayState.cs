﻿using System;
using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.Managers;
using zbrozonoidLibrary.States.BallInPlayCommands;

namespace zbrozonoidEngine.States
{
    public class BallInPlayState : IBallState
    {
        private Game game;

        private readonly IBallInPlayCommand handleScreenCollisionCommand;
        private readonly IBallInPlayCommand handleBorderCollisionCommand;
        private readonly IBallInPlayCommand handlePadCollisionCommand;
        private readonly IBallInPlayCommand handleBrickCollisionCommand;

        private List<IBrick> BricksHitList => game.BricksHitList;
        private readonly List<IBallInPlayCommand> collisionCommands;

        public BallInPlayState(Game game)
        {
            this.game = game;

            handleScreenCollisionCommand = new HandleScreenCollisionCommand(game);
            handleBorderCollisionCommand = new HandleBorderCollisionCommand(game);
            handlePadCollisionCommand = new HandlePadCollisionCommand(game);
            handleBrickCollisionCommand = new HandleBrickCollisionCommand(game, handleBorderCollisionCommand);

            collisionCommands = new List<IBallInPlayCommand>() { 
                handleScreenCollisionCommand,
                handleBorderCollisionCommand,
                handlePadCollisionCommand,
                handleBrickCollisionCommand
                };
        }

        public bool action(IBall ball)
        {
            ball.MoveBall();

            foreach (var command in collisionCommands)
            {
                if (!command.Execute(ball))
                {
                    game.SavePosition(ball);
                    return false;
                }
            }

            game.SavePosition(ball);
            return true;
        }

    }
}
