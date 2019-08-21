using System;
namespace zbrozonoid.Views
{
    public class GamePlayView : IGameView
    {
        private IDrawGameObjects draw;

        public GamePlayView(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawLifesAndScoresInfo();
            draw.DrawBallFasterTimer();
        }
    }
}
