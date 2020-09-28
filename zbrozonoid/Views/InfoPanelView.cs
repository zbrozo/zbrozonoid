using System.Linq;
using SFML.Graphics;
using zbrozonoid.Views.Interfaces;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Views
{
    public class InfoPanelView : IInfoPanelView
    {
        private IGame game;
        private IRenderProxy render;

        private Text LiveAndScoresMessage
        {
            get { return PrepareLifesAndScoresMessage(); }
        }

        private Text FasterBallMessage
        {
            get { return PrepareFasterBallMessage(); }
        }

        private Text FireBallMessage
        {
            get { return PrepareFireBallMessage(); }
        }

        public InfoPanelView(IRenderProxy render, IGame game)
        {
            this.game = game;
            this.render = render;
        }

        public void Display()
        {
            DrawLifesAndScoresInfo();
            DrawFasterBallTimer();
            DrawFireBallTimer();
        }

        public void Dispose()
        {
        }

        private void DrawLifesAndScoresInfo()
        {
            render.Draw(LiveAndScoresMessage);
        }

        private void DrawFasterBallTimer()
        {
            if (FasterBallMessage.DisplayedString.Length > 0)
            {
                render.Draw(FasterBallMessage);
            }
        }

        private void DrawFireBallTimer()
        {
            if (FireBallMessage.DisplayedString.Length > 0)
            {
                render.Draw(FireBallMessage);
            }
        }

        private Text PrepareLifesAndScoresMessage()
        {
            const uint charSize = 20;
            int lifes = game.GameState.Lifes >= 0 ? game.GameState.Lifes : 0;
            int scores = game.GameState.Scores;
            return render.PrepareTextLine($"Lifes: {lifes}   Scores: {scores:D5}", 0, false, true, 20, 30, charSize);
        }

        private Text PrepareFasterBallMessage()
        {
            int value = game.FastBallCounter.GetValue();

            const uint charSize = 20;
            return render.PrepareTextLine($"FasterBall: {value}", 0, false, true, 800, 20, charSize);
        }

        private Text PrepareFireBallMessage()
        {
            int value = game.GameState.FireBallCountdown.Any() ? game.GameState.FireBallCountdown.Max(x => x.Value) : 0;

            const uint charSize = 20;
            return render.PrepareTextLine($"FireBall: {value}", 0, false, true, 800, 40, charSize);
        }
    }
}
