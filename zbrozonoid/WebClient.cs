using System;
using RestSharp;

namespace zbrozonoid
{
    public class WebClient
    {
        private readonly RestClient client;

        public WebClient(string url)
        {
            Console.Write(url);
            client = new RestClient(url);
        }

        public void Put(int id, string data)
        {
            var request = new RestRequest("values/" + id, Method.Put);
            request.AddStringBody(data, ContentType.Json);
            client.Execute(request);
        }

        public string Get(int id)
        {
            var request = new RestRequest("values/" + id, Method.Get);
            var response = client.Execute(request);
            return response.Content;
        }
    }
}
