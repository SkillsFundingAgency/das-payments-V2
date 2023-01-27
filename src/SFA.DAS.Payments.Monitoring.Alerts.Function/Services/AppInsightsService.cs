using System.Text.Json;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Alerts.Function.Helpers;
using SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients;

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
            var apiResultJsonString = await _client.GetAsJson<string>(linkToSearchResultsAPI);

            var options = new JsonSerializerOptions
            {
                Converters = 
                { 
                    new ObjectAsPrimitiveConverter() 
                },
                WriteIndented = true,
            };

            dynamic appInsightsResult = JsonSerializer.Deserialize<dynamic>(apiResultJsonString, options);

            return appInsightsResult;
        }
    }
}