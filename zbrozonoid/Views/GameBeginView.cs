using System;
namespace zbrozonoid.Views
{
    public class GameBeginView : IGameView
    {
        private IDrawGameObjects draw;
        private IView menuView;

        public GameBeginView(IView menuView, IDrawGameObjects draw)
        {
            this.draw = draw;
            this.menuView = menuView;
        }

        public void Action()
        {
            draw.DrawTitle();
            menuView.Display();
        }

    }
}
