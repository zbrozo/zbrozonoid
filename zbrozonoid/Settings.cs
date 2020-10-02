using System;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace zbrozonoid
{
    public class Settings
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const int MaxPlayers = 2;

        [JsonProperty("WebServiceAddress")]
        public string WebServiceAddress { get; set; } = "http://localhost:5000/api/";

        [JsonProperty("PlayerOneId")]
        public uint PlayerOneId { get; set; } = 1;

        [JsonProperty("PlayerTwoId")]
        public uint PlayerTwoId { get; set; } = 2;

        [JsonProperty("Remote")]
        public bool Remote { get; set; } = false;

        [JsonProperty("Players")]
        public uint Players { get; set; } = 1;

        public static Settings LoadSettings()
        {
            try
            {
                using (StreamReader file = new StreamReader(@"settings.json"))
                {
                    var json = file.ReadToEnd();
                    return JsonConvert.DeserializeObject<Settings>(json);
                }
            }
            catch (FileNotFoundException)
            {
                Logger.Warn("Settings file not found");
            }

            return null;
        }

        public static void ValidateSettings(Settings settings)
        {
            if (settings.Players > MaxPlayers)
            {
                throw new OutOfMemoryException();
            }
        }

        public static void SaveSettings(Settings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            using (StreamWriter file = new StreamWriter(@"settings.json"))
            {
                file.Write(json);
            }
        }
    }
}
