using System;
namespace zbrozonoid.Views
{
    public class GameBeginView : IView
    {
        private IView menuView;
        private IView titleView;

        public GameBeginView(ViewCommon viewCommon, IView menuView)
        {
            titleView = new TitleMessageView(viewCommon);
            this.menuView = menuView;
        }

        public void Display()
        {
            titleView.Display();
            menuView.Display();
        }

    }
}
