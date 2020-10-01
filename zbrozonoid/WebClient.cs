using RestSharp;

namespace zbrozonoid
{
    public class WebClient
    {
        private RestClient client = new RestClient("http://localhost:5000/api/");

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
