using System;
namespace zbrozonoid.Views
{
    public class GamePlayView : IView
    {
        private IDrawGameObjects draw;

        public GamePlayView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Display()
        {
            draw.DrawLifesAndScoresInfo();
            draw.DrawFasterBallTimer();
            draw.DrawFireBallTimer();
        }
    }
}
