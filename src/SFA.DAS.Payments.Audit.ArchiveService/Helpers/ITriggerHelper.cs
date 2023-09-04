using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Audit.ArchiveService.Helpers;

public interface ITriggerHelper
{
    Task<OrchestrationStatusQueryResult> GetRunningInstances(
        string orchestratorName,
        string instanceIdPrefix,
        IDurableOrchestrationClient starter,
        IPaymentLogger log);

    Task<HttpResponseMessage> StartOrchestrator(
        HttpRequestMessage req,
        IDurableOrchestrationClient starter,
        IPaymentLogger log,
        ITriggerHelper triggerHelper,
        string orchestratorName,
        string triggerName);
}