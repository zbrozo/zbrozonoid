using System;
namespace zbrozonoidLibrary.Interfaces
{
    public interface IGameState
    {
        int Lives { get; set; }

        int Scores { get; set; }

        int BallsOutOfScreen { get; set; }
    }
}
