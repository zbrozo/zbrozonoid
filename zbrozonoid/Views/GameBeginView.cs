using SFML.Graphics;
using zbrozonoid.Models;


namespace zbrozonoid.Views
{
    public class GameBeginView : IView
    {
        private GameBeginModel model = new GameBeginModel();

        private IView menuView;

        private IDrawGameObjects draw;

        private Text TitleMessage { get; set; }

        public GameBeginView(IDrawGameObjects draw, 
                             IView menuView)
        {
            this.draw = draw;
            this.menuView = menuView;
            TitleMessage = draw.PrepareTextLine.Prepare(model.GetTitle(), 0);
        }

        public void Display()
        {
            draw.Render.Draw(TitleMessage);
            menuView.Display();
        }

    }
}
