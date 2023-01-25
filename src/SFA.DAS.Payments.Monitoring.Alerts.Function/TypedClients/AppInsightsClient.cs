using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients
{
    public class AppInsightsClient : IAppInsightsClient
    {
        private readonly HttpClient _httpClient;

        public AppInsightsClient(HttpClient httpClient) 
        {
            _httpClient = httpClient;
        }

        public async Task<TResponse> Get<TResponse>(string requestUrl)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonSerializer.Deserialize<TResponse>(json);
        }
    }
}