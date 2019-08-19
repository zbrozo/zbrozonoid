﻿using zbrozonoidEngine;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Managers;

namespace zbrozonoidLibrary.States.BallInPlayCommands
{
    public class HandlePadCollisionCommand : IBallInPlayCommand
    {
        private Game game;

        public bool CollisionResult { set; get; }

        private readonly ICollisionManager collisionManagerForMoveReversion;

        public HandlePadCollisionCommand(Game game)
        {
            this.game = game;
            collisionManagerForMoveReversion = new CollisionManager();
        }

        public bool Execute(IBall ball)
        {
            CollisionResult = HandlePadCollision(ball);
            return !CollisionResult;
        }

        protected bool HandlePadCollision(IBall ball)
        {
            foreach (IPad pad in game.PadManager)
            {
                if (game.CollisionManager.Detect(pad, ball))
                {
                    pad.LogData();

                    CorrectBallPosition(pad, ball);
                    game.CollisionManager.Bounce(pad, ball);

                    ball.LogData();
                    return true;
                }
            }
            return false;
        }

        private void CorrectBallPosition(IPad pad, IBall ball)
        {
            Vector2 previous = ball.Boundary.Min;
            while (collisionManagerForMoveReversion.Detect(pad, ball))
            {
                if (!ball.MoveBall(true))
                {
                    ball.Boundary.Min = previous;
                    game.RestartBallYPosition(pad, ball);
                    return;
                }

                previous = ball.Boundary.Min;
                ball.SavePosition();

                foreach (IBorder border in game.BorderManager)
                {
                    if (collisionManagerForMoveReversion.Detect(border, ball))
                    {
                        game.SetBallStartPosition(pad, ball);
                        break;
                    }
                }

                if (game.ScreenCollisionManager.DetectAndVerify(ball))
                {
                    game.SetBallStartPosition(pad, ball);
                    break;
                }

            }

            ball.Boundary.Min = previous;

        }

    }
}