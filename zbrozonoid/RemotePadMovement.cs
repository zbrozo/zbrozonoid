using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using zbrozonoid.AppSettings;

namespace zbrozonoid
{
    public class RemotePadMovement
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Settings settings;
        private readonly WebClient webClient;

        private int GetWebId(int playerNr) => (int)settings?.Players.PlayersDetails.First(x => x.Nr == playerNr).WebId;

        private readonly object webLock = new object();

        public RemotePadMovement(Settings settings)
        {
            this.settings = settings;
            webClient = new WebClient(settings.WebServiceAddress);
        }

        public void PutAndGet(int movement, uint manipulatorId, Action<int, uint> setPadMove)
        {
            PadMovement result = null;
            lock (webLock)
            {
                int webId = GetWebId(1);
                var movementJson = JsonConvert.SerializeObject(new PadMovement { PlayerId = webId, Move = movement });
                webClient.Put(webId, movementJson);

                var response = webClient.Get(GetWebId(2));
                if (response != null && response.Result != null)
                {
                    var padMovement = JsonConvert.DeserializeObject<PadMovement>(response.Result);
                    if (padMovement == null)
                    {
                        Logger.Error("Remote paddle movement not received");
                    }
                    else
                    {
                        result = padMovement;
                    }
                }
            }


            if (result != null)
            {
                setPadMove(result.Move, manipulatorId);
            }
        }
    }
}
