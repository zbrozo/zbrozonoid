using System;
using zbrozonoidLibrary.Interfaces;
using zbrozonoidLibrary.Interfaces.States;
using zbrozonoidLibrary.States;

namespace zbrozonoidLibrary
{

    public class BallStateMachine
    {
        private BallInGameState ballInGameState;
        private BallInMenuState ballInMenuState;

        IBallState currentState;

        public BallStateMachine(Game game, IPadManager padManager)
        {
            ballInGameState = new BallInGameState(game);
            ballInMenuState = new BallInMenuState(game, padManager);

            currentState = ballInMenuState;
        }

        public bool action(IBall ball)
        {
            return currentState.action(ball);
        }

        public void goToInMenu()
        {
            currentState = ballInMenuState;
        }

        public void goToInGame()
        {
            currentState = ballInGameState;
        }

    }
}
