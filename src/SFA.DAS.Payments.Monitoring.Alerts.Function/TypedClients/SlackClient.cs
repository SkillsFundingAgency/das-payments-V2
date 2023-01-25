using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients
{
    public class SlackClient : ISlackClient
    {
        private readonly HttpClient _httpClient;

        public SlackClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsJson(string requestUrl, string jsonPayload)
        {
            var response = await _httpClient.PostAsJsonAsync(requestUrl, jsonPayload);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}