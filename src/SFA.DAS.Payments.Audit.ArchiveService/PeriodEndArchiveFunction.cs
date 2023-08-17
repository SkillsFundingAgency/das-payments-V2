using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.DataFactory;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

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
            var result = await context.CallActivityAsync<bool>(nameof(PeriodEndArchive), null);


            return true;
        }
        catch (Exception ex)
        {
        }

        return false;
    }

    [FunctionName(nameof(PeriodEndArchive))]
    public static async Task<bool> PeriodEndArchive()
    {
        //May not be needed
        var cred = new DefaultAzureCredential();
        // authenticate your client
        var client = new ArmClient(cred);

        var dataFactoryPipelineResourceId =
            DataFactoryPipelineResource.CreateResourceIdentifier(SubscriptionId, ResourceGroup,
                AzureDataFactoryName,
                PipeLine);
        var dataFactoryPipeline = client.GetDataFactoryPipelineResource(dataFactoryPipelineResourceId);

        // invoke the operation
        var parameterValueSpecification = new Dictionary<string, BinaryData>();
        string referencePipelineRunId = null;
        var result =
            await dataFactoryPipeline.CreateRunAsync(parameterValueSpecification, referencePipelineRunId);

        Console.WriteLine($"Succeeded: {result}");

        return true;
    }
}