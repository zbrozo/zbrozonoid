using System.Collections.Generic;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.Menu.Items
{
    public class PlayersMenuItem : IMenuItem
    {
        IGameConfig config;

        public PlayersMenuItem(IGameConfig config)
        {
            this.config = config;
        }

        public string Name => "Players: " + Options[config.Players];

        public Dictionary<int, string> Options = new Dictionary<int, string>() { 
                { 1, "One" },
                { 2, "Two" }
            };

        public void Execute()
        {
            if (!Options.ContainsKey(++config.Players))
            {
                const int OnePlayer = 1;

                config.Players = OnePlayer;
            }
        }
    }
}
