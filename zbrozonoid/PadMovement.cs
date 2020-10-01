using Newtonsoft.Json;

namespace zbrozonoid
{
    public class PadMovement
    {
        [JsonProperty("playerId")]
        public int PlayerId;

        [JsonProperty("movement")]
        public int Move;
    }
}
