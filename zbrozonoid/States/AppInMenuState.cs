using System;
namespace zbrozonoid.States
{
    public class AppInMenuState : IAppState
    {
        private IDrawGameObjects draw;

        public AppInMenuState(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawPressPlayToPlay();
        }

    }
}
