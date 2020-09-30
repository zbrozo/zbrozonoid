﻿using System;
using System.Collections.Generic;
using Autofac;
using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;
using zbrozonoidEngine.States;

namespace zbrozonoidEngine
{

    public class BallStateMachine
    {
        private readonly BallInPlayState ballInPlayState;
        private readonly BallInIdleState ballInIdleState;

        IBallState currentState;

        public BallStateMachine(
            ILifetimeScope scope, 
            ICollection<IBrick> bricks,
            //Action<IBall> savePosition,
            Action<IBall, IEnumerable<int>> handleBrickCollision,
            Action lostBalls)
        {
            ballInPlayState = new BallInPlayState(scope, bricks, handleBrickCollision, lostBalls);
            ballInIdleState = new BallInIdleState(scope);

            currentState = ballInIdleState;
        }

        public bool Action(IBall ball)
        {
            return currentState.action(ball);
        }

        public void GoIntoIdle()
        {
            currentState = ballInIdleState;
        }

        public void GoIntoPlay()
        {
            currentState = ballInPlayState;
        }

        public bool IsBallInIdleState()
        {
            return currentState is BallInIdleState;
        }
    }
}
