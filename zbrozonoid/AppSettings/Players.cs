using Newtonsoft.Json;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.AppSettings
{
    public class Players
    {
        [JsonProperty("PlayersAmount")]
        public int PlayersAmount { get; set; } = 1;

        [JsonProperty("PlayersDetails")]
        public Player[] PlayersDetails { get; set; } = new Player[]
            {
                new Player { Nr = 1, WebId = 1, Location = Edge.Bottom },
                new Player { Nr = 2, WebId = 2, Location = Edge.Top }
            };
    }
}
