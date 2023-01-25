using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public interface IAppInsightsService
    {
        Task<dynamic> GetAppInsightsSearchResults(string linkToSearchResultsAPI);
    }
}