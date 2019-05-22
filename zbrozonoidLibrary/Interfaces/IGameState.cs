using System;
namespace zbrozonoidLibrary.Interfaces
{
    public interface IGameState
    {
        bool ShouldGo { get; set; }

        int Lives { get; set; }

        int Scores { get; set; }

        int BallsOutOfScreen { get; set; }
    }
}
