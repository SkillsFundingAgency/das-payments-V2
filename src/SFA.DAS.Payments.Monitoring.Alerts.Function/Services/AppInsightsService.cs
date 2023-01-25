using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;
using System;
using System.Dynamic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public class AppInsightsService : IAppInsightsService
    {
        private readonly IAppInsightsClient _client;

        public AppInsightsService(IAppInsightsClient client)
        {
            _client = client;
        }

        public async Task<dynamic> GetAppInsightsSearchResults(string linkToSearchResultsAPI)
        {
            var appInsightsApiResultJson = await _client.Get<string>(linkToSearchResultsAPI);
            dynamic appInsightsResult = JsonSerializer.Deserialize<ExpandoObject>(appInsightsApiResultJson);

            return appInsightsResult;
        }
    }
}