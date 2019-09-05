using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class GameConfig : IGameConfig
    {
        public const int OnePlayer = 1;

        public int Players { get; set; } = OnePlayer;

        public int Mouses { get; set; }
    }
}
