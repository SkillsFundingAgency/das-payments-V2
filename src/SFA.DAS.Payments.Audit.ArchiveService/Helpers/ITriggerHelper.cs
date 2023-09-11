using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Audit.ArchiveService.Helpers;

public interface ITriggerHelper
{
    Task<HttpResponseMessage> StartOrchestrator(
        HttpRequestMessage req,
        IDurableOrchestrationClient starter,
        IPaymentLogger log,
        string orchestratorName,
        string triggerName);
}