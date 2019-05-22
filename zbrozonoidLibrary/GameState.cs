using System;
using zbrozonoidLibrary.Interfaces;

namespace zbrozonoidLibrary
{
    public class GameState : IGameState
    {
        public bool ShouldGo { get; set; } = false;

        public int Lives { get; set; } = -1;

        public int Scores { get; set; } = 0;

        public int BallsOutOfScreen { get; set; } = 0;

        public GameState()
        {
        }
    }
}
