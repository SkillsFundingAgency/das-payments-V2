using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService;

[DependencyInjectionConfig(typeof(DependencyRegister))]
public static class PeriodEndArchiveFunction
{
    private static readonly IPeriodEndArchiveConfiguration Config = new PeriodEndArchiveConfiguration();

    public static async Task<DataFactoryManagementClient> CreateClient(string applicationId, string tenantId,
        string authenticationKey, string subscriptionId)
    {
        // Authenticate and create a data factory management client
        var app = ConfidentialClientApplicationBuilder.Create(applicationId)
            .WithAuthority("https://login.microsoftonline.com/" + tenantId)
            .WithClientSecret(authenticationKey)
            .WithLegacyCacheCompatibility(false)
            .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
            .Build();

        var result = await app.AcquireTokenForClient(
                new[] { "https://management.azure.com//.default" })
            .ExecuteAsync();
        ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);

        return new DataFactoryManagementClient(cred)
        {
            SubscriptionId = subscriptionId
        };
    }

    public static async Task StartArchivePipeline(IPaymentLogger logger)
    {
        // Set variables
        var tenantId = "<your tenant ID>";
        var applicationId = "<your application ID>";
        var authenticationKey = "<your authentication key for the application>";
        var subscriptionId = "12f72527-6622-45d3-90a4-0a5d3644c45c";
        var resourceGroup = "DCOL-DAS-DataFactoryDAS-WEU";

        var dataFactoryName =
            "DCOL-DAS-DataFactoryDAS-WEU";
        var pipelineName = "CopyPaymentsToArchive";

        var client = await CreateClient(Config.ApplicationId, Config.TenantId, Config.AuthenticationKey,
            Config.SubscriptionId);

        // Create a pipeline run
        logger.LogInfo("Creating pipeline run...");

        var runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
            resourceGroup, dataFactoryName, pipelineName
        ).Result.Body;
        logger.LogInfo("Pipeline run ID: " + runResponse.RunId);

        PipelineRun pipelineRun;
        while (true)
        {
            pipelineRun = await client.PipelineRuns.GetAsync(
                resourceGroup, dataFactoryName, runResponse.RunId);
            logger.LogInfo("Period End Archive Status: " + pipelineRun.Status);
            if (pipelineRun.Status is "InProgress" or "Queued")
                Thread.Sleep(15000);
            else
                break;
        }

        var filterParams = new RunFilterParameters(
            DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
        var queryResponse = await client.ActivityRuns.QueryByPipelineRunAsync(
            resourceGroup, dataFactoryName, runResponse.RunId, filterParams);

        if (pipelineRun.Status != "Succeeded")
            logger.LogError(queryResponse.Value.First().Error.ToString());
        else
            logger.LogInfo(queryResponse.Value.First().Output.ToString());
    }

    public static async Task<string> MonitorArchivePipeline(string runId, [Inject] IPaymentLogger logger)
    {
        var client = await CreateClient(Config.ApplicationId, Config.TenantId, Config.AuthenticationKey,
            Config.SubscriptionId);

        // Monitor the pipeline run
        logger.LogInfo("Checking pipeline run status...");


        var pipelineRun = await client.PipelineRuns.GetAsync(Config.ResourceGroup, Config.AzureDataFactoryName, runId);
        logger.LogInfo("Status: " + pipelineRun.Status);

        return pipelineRun.Status;
    }
}