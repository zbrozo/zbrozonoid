using System;
namespace zbrozonoid.Views
{
    public class GameBeginView : IView
    {
        private IView menuView;
        //private IView titleView;
        IDrawGameObjects draw;

        public GameBeginView(IDrawGameObjects draw, IView menuView)
        {
            //titleView = new TitleMessageView(viewCommon);
            this.draw = draw;
            this.menuView = menuView;
        }

        public void Display()
        {
            draw.DrawTitle();
            //titleView.Display();
            menuView.Display();
        }

    }
}
