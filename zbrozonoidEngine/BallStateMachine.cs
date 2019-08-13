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

        public BallStateMachine(Game game, 
                                IScreenCollisionManager screenCollisionManager,
                                ICollisionManager collisionManager,
                                IPadManager padManager,
                                IBorderManager borderManager,
                                ILevelManager levelManager)
        {
            ballInPlayState = new BallInPlayState(game, screenCollisionManager, collisionManager, levelManager);
            ballInIdleState = new BallInIdleState(game, padManager);

            currentState = ballInIdleState;
        }

        public bool action(IBall ball)
        {
            return currentState.action(ball);
        }

        public void goIntoIdle()
        {
            currentState = ballInIdleState;
        }

        public void goIntoPlay()
        {
            currentState = ballInPlayState;
        }

        public bool IsBallInIdleState()
        {
            return currentState is BallInIdleState;
        }
    }
}
