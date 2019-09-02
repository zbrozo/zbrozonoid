using zbrozonoidEngine.Interfaces;
using zbrozonoidEngine.Interfaces.States;

namespace zbrozonoidEngine.States
{
    public class BallInIdleState : IBallState
    {
        private readonly IGame game;
        private readonly IPadManager padManager; 

        public BallInIdleState(IGame game)
        {
            this.game = game;
            this.padManager = game.PadManager;
        }

        public bool action(IBall ball)
        {
            IPad pad = padManager.GetFirst();
            padManager.SetBallStartPosition(pad, ball);
            return true;
        }
    }
}
