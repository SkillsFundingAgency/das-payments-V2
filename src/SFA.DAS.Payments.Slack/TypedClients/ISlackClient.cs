using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Slack.TypedClients
{
    public interface ISlackClient
    {
        Task<HttpResponseMessage> PostAsJsonAsync(string requestUrl, object jsonPayload);
    }
}