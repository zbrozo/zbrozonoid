using SFML.Graphics;
using zbrozonoid.Views.Interfaces;

namespace zbrozonoid.Views
{
    public class GameOverView : IGameOverView
    {
        private IRenderProxy render;
        private IView playfieldView;

        private readonly Text GameOverMessage;

        public GameOverView(IRenderProxy render,
                            IGamePlayfieldView playfieldView)
        {
            this.render = render;
            this.playfieldView = playfieldView;

            GameOverMessage = PrepareGameOverMessage();
        }

        public void Display()
        {
            playfieldView.Display();
            render.Draw(GameOverMessage);
        }

        public void Dispose()
        {
        }

        private Text PrepareGameOverMessage()
        {
            return render.PrepareTextLine("game over", 4);
        }

    }
}
