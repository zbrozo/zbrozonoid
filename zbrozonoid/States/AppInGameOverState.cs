using System;
namespace zbrozonoid.States
{
    public class AppInGameOverState : IAppState
    {
        private IDrawGameObjects draw;

        public AppInGameOverState(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawGameOver();
        }

    }
}
