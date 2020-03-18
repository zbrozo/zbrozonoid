using System;
namespace zbrozonoid.Views
{
    public class GameOverView : IView
    {
        private IDrawGameObjects draw;

        public GameOverView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Display()
        {
            draw.DrawGameOver();
        }

        public void Dispose()
        {
        }
    }
}
