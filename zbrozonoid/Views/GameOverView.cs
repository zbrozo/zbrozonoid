using System;
namespace zbrozonoid.Views
{
    public class GameOverView : IGameView
    {
        private IDrawGameObjects draw;

        public GameOverView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawGameOver();
        }

    }
}
