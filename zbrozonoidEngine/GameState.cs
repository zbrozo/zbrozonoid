using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class GameState : IGameState
    {
        public bool Pause { get; set; } = false;

        public int Lifes { get; set; } = -1;

        public int Scores { get; set; } = 0;
    }
}
