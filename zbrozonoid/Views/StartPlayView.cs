using System;
namespace zbrozonoid.Views
{
    public class StartPlayView : IView
    {
        private IDrawGameObjects draw;

        public StartPlayView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Display()
        {
            draw.DrawPressPlayToPlay();
        }
    }
}
