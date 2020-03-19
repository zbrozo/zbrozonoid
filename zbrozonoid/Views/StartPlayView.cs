using SFML.Graphics;

namespace zbrozonoid.Views
{
    public class StartPlayView : IView
    {
        private IRenderProxy render;

        private readonly Text PressButtonToPlayMessage;


        public StartPlayView(IRenderProxy render)
        {
            this.render = render;

            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
        }

        public void Display()
        {
            render.Draw(PressButtonToPlayMessage);
        }

        public void Dispose()
        {
        }

        private Text PreparePressButtonToPlayMessage()
        {
            return render.PrepareTextLine("Press mouse button to play", 4);
        }


    }
}
