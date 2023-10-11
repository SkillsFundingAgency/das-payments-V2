using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Slack.TypedClients
{
    public class SlackClient : ISlackClient
    {
        private readonly HttpClient _httpClient;

        public SlackClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, object jsonPayload)
        {
            if (string.IsNullOrEmpty(requestUrl)) 
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            if (jsonPayload == null) 
            {
                throw new ArgumentNullException(nameof(jsonPayload));
            }

            var response = await _httpClient.PostAsJsonAsync(requestUrl, jsonPayload);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}