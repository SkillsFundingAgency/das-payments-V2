using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

        public async Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, string jsonPayload)
        {
            if (string.IsNullOrEmpty(requestUrl)) 
            {
                throw new ArgumentNullException(nameof(requestUrl));
            }

            if (jsonPayload == null) 
            {
                throw new ArgumentNullException(nameof(jsonPayload));
            }

            var requestContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, requestContent);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new ArgumentException($"Slack API returned HTTP 400 Bad Request : {responseContent}");
            }

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}