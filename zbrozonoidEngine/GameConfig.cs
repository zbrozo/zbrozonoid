using System;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoidEngine
{
    public class GameConfig : IGameConfig
    {
        public int Players { get; set; } = 1;

    }
}
