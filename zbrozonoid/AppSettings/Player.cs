using Newtonsoft.Json;
using zbrozonoidEngine.Interfaces;

namespace zbrozonoid.AppSettings
{
    public class Player
    {
        [JsonProperty("Nr")]
        public int Nr;

        [JsonProperty("WebId")]
        public int WebId;

        [JsonProperty("Location")]
        public Edge Location;
    }
}
