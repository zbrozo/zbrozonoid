using System;
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

        public BallStateMachine(IGame game)
        {
            ballInPlayState = new BallInPlayState(game);
            ballInIdleState = new BallInIdleState(game);

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
