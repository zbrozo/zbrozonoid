using SFML.Graphics;
using zbrozonoid.Views.Interfaces;

namespace zbrozonoid.Views
{
    public class StartPlayView : IStartPlayView
    {
        private IRenderProxy render;
        private IView playfieldView;

        private readonly Text PressButtonToPlayMessage;

        public StartPlayView(IRenderProxy render,
                             IGamePlayfieldView playfieldView)
        {
            this.render = render;
            this.playfieldView = playfieldView;

            PressButtonToPlayMessage = PreparePressButtonToPlayMessage();
        }

        public void Display()
        {
            playfieldView.Display();
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
