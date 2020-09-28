namespace zbrozonoidEngine.Interfaces
{
    public interface IGameState
    {
        bool Pause { get; set; }

        int Lifes { get; set; }

        int Scores { get; set; }
    }
}
