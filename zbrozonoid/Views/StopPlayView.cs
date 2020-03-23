using SFML.Graphics;
using zbrozonoid.Views.Interfaces;

namespace zbrozonoid.Views
{
    public class StopPlayView : IStopPlayView
    {
        private IRenderProxy render;
        private readonly Text StopPlayMessage;

        public StopPlayView(IRenderProxy render)
        {
            this.render = render;
            StopPlayMessage = PrepareStopPlayMessage();
        }

        public void Display()
        {
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
