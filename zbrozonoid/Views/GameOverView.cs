using SFML.Graphics;

namespace zbrozonoid.Views
{
    public class GameOverView : IView
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
