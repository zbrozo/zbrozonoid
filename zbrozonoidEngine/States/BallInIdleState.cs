using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;

namespace zbrozonoidEngine.States
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
