using System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class GameState : IGameState
    {
        public int Lifes { get; set; } = -1;

        public int Scores { get; set; } = 0;

        public int BallsOutOfScreen { get; set; } = 0;

        public GameState()
        {
        }
    }
}
