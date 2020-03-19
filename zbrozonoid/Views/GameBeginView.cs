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
        private IRenderProxy render;

        private Text TitleMessage { get; set; }

        public GameBeginView(IRenderProxy render,
                             IGamePlayfieldView playfieldView,
                             IMenuView menuView)
        {
            this.render = render;
            this.menuView = menuView;
            this.playfieldView = playfieldView;
            TitleMessage = render.PrepareTextLine(model.GetTitle(), 0);
        }

        public void Display()
        {
            playfieldView.Display();
            render.Draw(TitleMessage);
            menuView.Display();
        }

        public void Dispose()
        {
        }
    }
}
