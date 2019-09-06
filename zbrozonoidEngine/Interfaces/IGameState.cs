using System.Collections.Generic;

namespace zbrozonoidEngine.Interfaces
{
    public interface IGameState
    {
        bool Pause { get; set; }

        int Lifes { get; set; }

        int Scores { get; set; }

        int BallsOutOfScreen { get; set; }

        Dictionary<IBall, int> FasterBallCountdown { get; set; }

        Dictionary<ITail, int> FireBallCountdown { get; set; }
    }
}
