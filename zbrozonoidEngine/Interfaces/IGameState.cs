using System.Collections.Generic;

namespace zbrozonoidEngine.Interfaces
{
    public interface IGameState
    {
        bool Pause { get; set; }

        int Lifes { get; set; }

        int Scores { get; set; }

        Dictionary<IBall, int> FasterBallCountdown { get; }

        Dictionary<ITail, int> FireBallCountdown { get; }
    }
}
