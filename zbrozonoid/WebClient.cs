using System;
using System.Threading.Tasks;
using RestSharp;

namespace zbrozonoid
{
    public class WebClient
    {
        private readonly RestClient client;

        public WebClient(string url)
        {
            var options = new RestClientOptions(url);
            //  options.Timeout = new TimeSpan(300);
            client = new RestClient(options);
        }

        public async void Put(int id, string data)
        {
            var request = new RestRequest("values/" + id, Method.Put);
            request.AddStringBody(data, ContentType.Json);
            //request.Timeout = new TimeSpan(6000);
            await client.ExecuteAsync(request);
        }

        public async Task<string> Get(int id)
        {
            var request = new RestRequest("values/" + id, Method.Get)
            {
                //Timeout = new TimeSpan(1000)
            };
            var response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }
}
