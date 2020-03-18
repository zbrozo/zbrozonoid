using System;
namespace zbrozonoid.Views
{
    public class StopPlayView : IView
    {
        private IDrawGameObjects draw;

        public StopPlayView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Display()
        {
            draw.DrawStopPlayMessage();
        }

        public void Dispose()
        {
        }
    }
}
