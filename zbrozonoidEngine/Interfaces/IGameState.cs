using System.Collections.Generic;

namespace zbrozonoidEngine.Interfaces
{
    public interface IGameState
    {
        int Lifes { get; set; }

        int Scores { get; set; }

        int BallsOutOfScreen { get; set; }

        Dictionary<IBall, int> BallFasterCountdown { get; set; }
    }
}
