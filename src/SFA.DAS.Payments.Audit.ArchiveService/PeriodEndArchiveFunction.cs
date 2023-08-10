using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.Audit.ArchiveService;

public static class PeriodEndArchiveFunction
{
    [FunctionName("PeriodEndArchive")]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>
        {
            // Replace "hello" with the name of your Durable Activity Function.
            await context.CallActivityAsync<string>(nameof(PeriodEndArchive), "Tokyo")
        };

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

    [FunctionName(nameof(PeriodEndArchive))]
    public static string PeriodEndArchive([ActivityTrigger] string name, ILogger log)
    {
        log.LogInformation($"Saying hello to {name}.");
        return $"Hello {name}!";
    }

    [FunctionName("PeriodEndArchive_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        // Function input comes from the request content.
        var instanceId = await starter.StartNewAsync("PeriodEndArchive");

        log.LogInformation($"Started PeriodEndArchive orchestration with ID = '{instanceId}'.");

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}