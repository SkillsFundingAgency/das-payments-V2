using System;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService;

[DependencyInjectionConfig(typeof(DependencyRegister))]
public static class PeriodEndArchiveFunction
{
    private static readonly IPeriodEndArchiveConfiguration Config = new PeriodEndArchiveConfiguration();

    [FunctionName("PeriodEndArchive_Orchestrator")]
    public static async Task<bool> PeriodEndArchive_Orchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var result = await context.CallActivityAsync<string>(nameof(PeriodEndArchive), null);
        return true;
    }

    [FunctionName("PeriodEndArchive")]
    public static async Task<string> PeriodEndArchive()
    {
        var httpClient = new HttpClient();
        var pipelineRunUrl =
            $"https://management.azure.com/subscriptions/" +
            $"{Config.SubscriptionId}/resourceGroups/" +
            $"{Config.ResourceGroup}/providers/Microsoft.DataFactory/factories/" +
            $"{Config.AzureDataFactoryName}/pipelines/" +
            $"{Config.PipeLine}/createRun?api-version=2018-06-01";

        var result = await httpClient.PostAsync(pipelineRunUrl, null);

        Console.WriteLine($"Succeeded: {result}");
        return "result";
    }

    /*[FunctionName("PeriodEndArchive_CounterHttpStart")]
    public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function)]
        HttpRequestMessage req,
        [DurableClient] IDurableClient client,
        string entityKey,
        [Inject] IPaymentLogger logger)
    {
        if (req.Method == HttpMethod.Post)
        {
            await client.StartNewAsync("PeriodEndArchive_Orchestrator");
            await client.SignalEntityAsync(entityId, "add", 1);
            return req.CreateResponse(HttpStatusCode.OK);
        }

        var stateResponse = await client.ReadEntityStateAsync<JToken>(entityId);
        return req.CreateResponse(HttpStatusCode.OK, stateResponse.EntityState);
    }*/

    [FunctionName("PeriodEndArchive_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        [Inject] IPaymentLogger logger)
    {
        // Function input comes from the request content.
        var instanceId = await starter.StartNewAsync(nameof(PeriodEndArchive_Orchestrator));
        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}