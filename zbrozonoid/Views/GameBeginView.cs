using SFML.Graphics;
using zbrozonoid.Menu;
using zbrozonoid.Models;

namespace zbrozonoid.Views
{
    public class GameBeginView : IView
    {
        private GameBeginModel model = new GameBeginModel();

        private IView menuView;
        private IView playfieldView;

        private IDrawGameObjects draw;

        private Text TitleMessage { get; set; }

        public GameBeginView(
                             IDrawGameObjects draw,
                             IGamePlayfieldView playfieldView,
                             IMenuView menuView)
        {
            this.draw = draw;
            this.menuView = menuView;
            this.playfieldView = playfieldView;
            TitleMessage = draw.PrepareTextLine.Prepare(model.GetTitle(), 0);
        }

        public void Display()
        {
            playfieldView.Display();

            draw.Render.Draw(TitleMessage);
            menuView.Display();
        }

        public void Dispose()
        {
        }
    }
}
