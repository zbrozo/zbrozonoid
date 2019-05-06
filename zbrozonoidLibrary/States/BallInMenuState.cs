using zbrozonoidLibrary.Interfaces;
using zbrozonoidLibrary.Interfaces.States;

namespace zbrozonoidLibrary.States
{
    public class BallInMenuState : IBallState
    {
        private Game game;
        private IPadManager padManager; 

        public BallInMenuState(Game game, IPadManager padManager)
        {
            this.game = game;
            this.padManager = padManager;
        }

        public bool action(IBall ball)
        {
            if (!game.ShouldGo)
            {
                IPad pad = padManager.GetFirst();
                game.SetBallStartPosition(pad, ball);
            }
            return true;
        }
    }
}
