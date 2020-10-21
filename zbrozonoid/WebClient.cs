using RestSharp;

namespace zbrozonoid
{
    public class WebClient
    {
        private readonly RestClient client;

        public WebClient(string url)
        {
            client = new RestClient(url);
        }

        public void Put(int id, string data)
        {
            var request = new RestRequest("values/" + id, Method.PUT);
            request.AddJsonBody(data);
            var response = client.Execute(request);
            var status = response.StatusCode;
        }

        public string Get(int id)
        {
            var request = new RestRequest("values/" + id, Method.GET);
            IRestResponse<string> response = client.Execute<string>(request);
            client.Execute(request);
            return response.Content;
        }
    }
}
