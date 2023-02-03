using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients
{
    public interface IAppInsightsClient
    {
        public Task<dynamic> GetSearchResultsAsync(string requestUrl);
    }
}