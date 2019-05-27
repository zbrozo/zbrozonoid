using System;
namespace zbrozonoid.Views
{
    public class StartPlayView : IGameView
    {
        private IDrawGameObjects draw;

        public StartPlayView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawPressPlayToPlay();
        }
    }
}
