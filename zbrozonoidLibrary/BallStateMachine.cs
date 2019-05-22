using System;
using zbrozonoidLibrary.Interfaces;
using zbrozonoidLibrary.Interfaces.States;
using zbrozonoidLibrary.States;

namespace zbrozonoidLibrary
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
            ballInPlayState = new BallInPlayState(game, screenCollisionManager, collisionManager, padManager, borderManager, levelManager);
            ballInIdleState = new BallInIdleState(game, padManager);

            currentState = ballInIdleState;
        }

        public bool action(IBall ball)
        {
            return currentState.action(ball);
        }

        public void goToInMenu()
        {
            currentState = ballInIdleState;
        }

        public void goToInGame()
        {
            currentState = ballInPlayState;
        }

    }
}
