using SFML.Graphics;
using zbrozonoid.Views.Interfaces;

namespace zbrozonoid.Views
{
    public class StopPlayView : IStopPlayView
    {
        private IRenderProxy render;
        private IView playfieldView;

        private readonly Text StopPlayMessage;

        public StopPlayView(IRenderProxy render,
                            IGamePlayfieldView playfieldView)
        {
            this.render = render;
            this.playfieldView = playfieldView;

            StopPlayMessage = PrepareStopPlayMessage();
        }

        public void Display()
        {
            playfieldView.Display();
            render.Draw(StopPlayMessage);
        }

        public void Dispose()
        {
        }

        private Text PrepareStopPlayMessage()
        {
            return render.PrepareTextLine("Stop play (y/n)", 4);
        }

    }
}
