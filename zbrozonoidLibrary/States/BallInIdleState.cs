using zbrozonoidLibrary.Interfaces;
using zbrozonoidLibrary.Interfaces.States;

namespace zbrozonoidLibrary.States
{
    public class BallInIdleState : IBallState
    {
        private Game game;
        private IPadManager padManager; 

        public BallInIdleState(Game game, IPadManager padManager)
        {
            this.game = game;
            this.padManager = padManager;
        }

        public bool action(IBall ball)
        {
            IPad pad = padManager.GetFirst();
            game.SetBallStartPosition(pad, ball);
            return true;
        }
    }
}
