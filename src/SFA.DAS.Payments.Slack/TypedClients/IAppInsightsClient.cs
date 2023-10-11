using System.Threading.Tasks;

namespace SFA.DAS.Payments.Slack.TypedClients
{
    public interface IAppInsightsClient
    {
        Task<dynamic> GetSearchResultsAsync(string requestUrl);
    }
}