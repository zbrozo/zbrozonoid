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
        private readonly IScreenCollisionManager screenCollisionManager;
        private readonly ICollisionManager collisionManager;
        private readonly IPadManager padManager;
        private readonly IBorderManager borderManager;
        private readonly ICollisionManager collisionManagerForMoveReversion;
        private readonly ILevelManager levelManager;

        private readonly IBallInPlayCommand handleScreenCollisionCommand;
        private readonly IBallInPlayCommand handleBorderCollisionCommand;
        private readonly IBallInPlayCommand handlePadCollisionCommand;

        private List<IBrick> BricksHitList => game.BricksHitList;
        private readonly List<IBallInPlayCommand> commands;

        public BallInPlayState(Game game)
        {
            this.game = game;
            this.collisionManager = game.CollisionManager;
            this.levelManager = game.LevelManager;
            this.screenCollisionManager = game.ScreenCollisionManager;
            this.padManager = game.PadManager;
            this.borderManager = game.BorderManager;

            handleScreenCollisionCommand = new HandleScreenCollisionCommand(game);
            handleBorderCollisionCommand = new HandleBorderCollisionCommand(game);
            handlePadCollisionCommand = new HandlePadCollisionCommand(game);

            commands = new List<IBallInPlayCommand>() { 
                handleScreenCollisionCommand,
                handleBorderCollisionCommand,
                handlePadCollisionCommand
                };

            collisionManagerForMoveReversion = new CollisionManager();
        }

        public bool action(IBall ball)
        {
            ball.MoveBall();

            foreach (var command in commands)
            {
                if (!command.Execute(ball))
                {
                    return false;
                }
            }

            if (HandleBrickCollision(ball, handleBorderCollisionCommand.CollisionResult))
            {
                game.SavePosition(ball);
                return false;
            }

            game.SavePosition(ball);
            return true;
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
                    collisionManager.Bounce(BricksHitList, BricksHitList[0], ball);
                }

                return true;
            }

            return false;
        }

        private bool DetectBrickCollision(IBall ball, out List<BrickHit> bricksHit)
        {
            bricksHit = new List<BrickHit>();
            List<IBrick> bricks = levelManager.GetCurrent().Bricks;

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

                if (collisionManager.Detect(brick, ball))
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
