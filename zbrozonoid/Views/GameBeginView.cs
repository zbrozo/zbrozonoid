namespace zbrozonoid.Views
{
    public class GameBeginView : IView
    {
        private IView menuView;
        IDrawGameObjects draw;

        public GameBeginView(IDrawGameObjects draw, IView menuView)
        {
            this.draw = draw;
            this.menuView = menuView;
        }

        public void Display()
        {
            draw.DrawTitle();
            menuView.Display();
        }

    }
}
