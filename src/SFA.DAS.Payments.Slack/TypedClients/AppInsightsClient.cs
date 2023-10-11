using System;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.Payments.Slack.JsonHelpers;

namespace SFA.DAS.Payments.Slack.TypedClients
{
    public class AppInsightsClient : IAppInsightsClient
    {
        private readonly HttpClient _httpClient;
        private readonly IDynamicJsonDeserializer _deserializer;

        public AppInsightsClient(HttpClient httpClient, IDynamicJsonDeserializer deserializer) 
        {
            _httpClient = httpClient;
            _deserializer = deserializer;
        }

        public async Task<dynamic> GetSearchResultsAsync(string requestUrl)
        {
            if (string.IsNullOrEmpty(requestUrl))
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            var response = await _httpClient.GetAsync(requestUrl).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            var result = _deserializer.Deserialize(json);

            return result;
        }
    }
}