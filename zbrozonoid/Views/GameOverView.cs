using SFML.Graphics;
using zbrozonoid.Views.Interfaces;

namespace zbrozonoid.Views
{
    public class GameOverView : IGameOverView
    {
        private IRenderProxy render;
        private readonly Text GameOverMessage;
        public GameOverView(IRenderProxy render)
        {
            this.render = render;
            GameOverMessage = PrepareGameOverMessage();
        }

        public void Display()
        {
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
