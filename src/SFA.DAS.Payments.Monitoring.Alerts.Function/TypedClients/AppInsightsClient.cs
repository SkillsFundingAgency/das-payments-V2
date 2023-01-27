using System;
using System.Net.Http;
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

        public async Task<string> GetAsJson<TResponse>(string requestUrl)
        {
            if (string.IsNullOrEmpty(requestUrl))
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            var response = await _httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return json;
        }
    }
}