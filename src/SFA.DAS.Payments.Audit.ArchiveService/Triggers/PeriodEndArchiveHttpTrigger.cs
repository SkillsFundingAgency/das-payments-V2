using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;
using SFA.DAS.Payments.Audit.ArchiveService.Orchestrators;

namespace SFA.DAS.Payments.Audit.ArchiveService.Triggers;

[DependencyInjectionConfig(typeof(DependencyRegister))]
public static class PeriodEndArchiveHttpTrigger
{
    [FunctionName(nameof(PeriodEndArchiveHttpTrigger))]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "orchestrators/PeriodEndArchiveOrchestrator")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        [DurableClient] IDurableEntityClient client,
        [Inject] IPaymentLogger log
    )
    {
        if (req.Method == HttpMethod.Post)
        {
            ITriggerHelper triggerHelper = new TriggerHelper();
            return await triggerHelper.StartOrchestrator(req, starter, log, nameof(PeriodEndArchiveOrchestrator),
                nameof(PeriodEndArchiveHttpTrigger));
        }

        var entityId = new EntityId(nameof(HandleCurrentJobId.Handle), HandleCurrentJobId.PeriodEndArchiveEntityName);
        var stateResponse = await client.ReadEntityStateAsync<JObject>(entityId);
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(stateResponse), Encoding.UTF8, "application/json")
        };
        var response = req.CreateResponse(HttpStatusCode.OK, "{}");
        return response;
        return req.CreateResponse(HttpStatusCode.OK, stateResponse.EntityState);
    }
}