using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.DataFactory;
using Azure.ResourceManager.DataFactory.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Payments.Audit.ArchiveService;

public static class PeriodEndArchiveFunction
{
    private const string ResourceGroup = "DCOL-<Environment Abbreviation>-DataFactoryDAS-WEU";
    private const string AzureDataFactoryName = "DCOL-<Environment Abbreviation>-DataFactoryDAS-WEU";
    private const string PipeLine = "CopyPaymentsToArchive";
    private const string SubscriptionId = "subId";

    private const string PipelineRunUrl =
        $"https://management.azure.com/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroup}/providers/Microsoft.DataFactory/factories/{AzureDataFactoryName}/pipelines/{PipeLine}/createRun?api-version=2018-06-01";


    [FunctionName("PeriodEndArchive")]
    public static async Task<bool> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var httpClient = new HttpClient();

        try
        {
            // Generated from example definition: specification/datafactory/resource-manager/Microsoft.DataFactory/stable/2018-06-01/examples/Pipelines_CreateRun.json
            // this example is just showing the usage of "Pipelines_CreateRun" operation, for the dependent resources, they will have to be created separately.

            // get your azure access token, for more details of how Azure SDK get your access token, please refer to https://learn.microsoft.com/en-us/dotnet/azure/sdk/authentication?tabs=command-line
            var cred = new DefaultAzureCredential();
            // authenticate your client
            var client = new ArmClient(cred);

            // this example assumes you already have this DataFactoryPipelineResource created on azure
            // for more information of creating DataFactoryPipelineResource, please refer to the document of DataFactoryPipelineResource
            var dataFactoryPipelineResourceId =
                DataFactoryPipelineResource.CreateResourceIdentifier(SubscriptionId, ResourceGroup,
                    AzureDataFactoryName,
                    PipeLine);
            var dataFactoryPipeline = client.GetDataFactoryPipelineResource(dataFactoryPipelineResourceId);

            // invoke the operation
            IDictionary<string, BinaryData> parameterValueSpecification = new Dictionary<string, BinaryData>
            {
                ["OutputBlobNameList"] = BinaryData.FromObjectAsJson(new object[] { "exampleoutput.csv" })
            };
            string referencePipelineRunId = null;
            PipelineCreateRunResult result =
                await dataFactoryPipeline.CreateRunAsync(parameterValueSpecification, referencePipelineRunId);

            Console.WriteLine($"Succeeded: {result}");

            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
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