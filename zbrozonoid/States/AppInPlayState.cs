using System;
namespace zbrozonoid.States
{
    public class AppInPlayState : IAppState
    {
        private IDrawGameObjects draw;

        public AppInPlayState(IDrawGameObjects draw)
        {
            this.draw = draw;
        }

        public void Action()
        {
            draw.DrawLivesAndScoresInfo();
        }
    }
}
