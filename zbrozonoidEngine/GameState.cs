using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class GameState : IGameState
    {

        public int Lifes { get; set; } = -1;

        public int Scores { get; set; } = 0;

        public int BallsOutOfScreen { get; set; } = 0;

        public Dictionary<IBall, int> FasterBallCountdown { get; set; } = new Dictionary<IBall, int>();

        public Dictionary<ITail, int> FireBallCountdown { get; set; } = new Dictionary<ITail, int>();
    }
}
