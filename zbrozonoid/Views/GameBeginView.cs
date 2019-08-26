using System;
namespace zbrozonoid.Views
{
    public class GameBeginView : IGameView
    {
        private IDrawGameObjects draw;

        public GameBeginView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawTitle();
            //draw.DrawPressPlayToPlay();
            draw.DrawMenu();
        }

    }
}
