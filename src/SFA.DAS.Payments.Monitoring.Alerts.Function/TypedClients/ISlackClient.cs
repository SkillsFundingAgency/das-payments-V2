using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.TypedClients
{
    public interface ISlackClient
    {
        public Task<HttpResponseMessage> PostAsJson(string requestUrl, string jsonPayload);
    }
}