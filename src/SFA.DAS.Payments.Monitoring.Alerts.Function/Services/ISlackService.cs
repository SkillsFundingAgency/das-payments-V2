using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Services
{
    public interface ISlackService
    {
        Task PostSlackAlert(string appInsightsAlertPayload, string slackChannelUri);
    }
}