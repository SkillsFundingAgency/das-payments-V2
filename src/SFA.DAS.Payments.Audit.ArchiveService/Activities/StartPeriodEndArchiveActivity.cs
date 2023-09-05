using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities;

public static class StartPeriodEndArchiveActivity
{
    private static readonly IPeriodEndArchiveConfiguration Config = new PeriodEndArchiveConfiguration();

    public static async Task<DataFactoryManagementClient> CreateClient()
    {
        // Authenticate and create a data factory management client
        var app = ConfidentialClientApplicationBuilder.Create(Config.ApplicationId)
            .WithAuthority("https://login.microsoftonline.com/" + Config.TenantId)
            .WithClientSecret(Config.AuthenticationKey)
            .WithLegacyCacheCompatibility(false)
            .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
            .Build();

        var result = await app.AcquireTokenForClient(
                new[] { "https://management.azure.com//.default" })
            .ExecuteAsync();
        ServiceClientCredentials cred = new TokenCredentials(result.AccessToken);

        return new DataFactoryManagementClient(cred)
        {
            SubscriptionId = Config.SubscriptionId
        };
    }

    [FunctionName(nameof(StartPeriodEndArchiveActivity))]
    public static async Task Run([ActivityTrigger] IPaymentLogger logger,
        [DurableClient] IDurableEntityClient entityClient)
    {
        var sleepTimer = 15000;
        var currentJobId =
            new EntityId(nameof(HandleCurrentJobId.Handle), HandleCurrentJobId.PeriodEndArchiveEntityName);


        /*// Set variables
        var tenantId = "1a92889b-8ea1-4a16-8132-347814051567";
        var applicationId = "35d05f13-18ff-402a-8839-08fd3d60cf37";
        var authenticationKey = "1f13a132-1d08-410b-b797-4cb295bb7590";
        var subscriptionId = "12f72527-6622-45d3-90a4-0a5d3644c45c";
        var resourceGroup = "DCOL-DAS-DataFactoryDAS-WEU";

        var dataFactoryName =
            "DCOL-DAS-DataFactoryDAS-WEU";
        var pipelineName = "CopyPaymentsToArchive";*/

        var client = await CreateClient();

        // Create a pipeline run
        logger.LogInfo("Creating pipeline run...");

        var runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
            Config.ResourceGroup, Config.AzureDataFactoryName, Config.PipeLine
        ).Result.Body;
        logger.LogInfo("Pipeline run ID: " + runResponse.RunId);

        await entityClient.SignalEntityAsync(currentJobId, "add", new RunInformation
        {
            JobId = runResponse.RunId,
            Status = "Started"
        });

        PipelineRun pipelineRun;
        while (true)
        {
            pipelineRun = await client.PipelineRuns.GetAsync(
                Config.ResourceGroup, Config.AzureDataFactoryName, runResponse.RunId);

            logger.LogInfo("Period End Archive Status: " + pipelineRun.Status);
            if (pipelineRun.Status is "InProgress" or "Queued")
            {
                await entityClient.SignalEntityAsync(currentJobId, "add", new RunInformation
                {
                    JobId = runResponse.RunId,
                    Status = pipelineRun.Status
                });
                Thread.Sleep(sleepTimer);
            }
            else
            {
                break;
            }
        }

        var filterParams = new RunFilterParameters(
            DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
        var queryResponse = await client.ActivityRuns.QueryByPipelineRunAsync(
            Config.ResourceGroup, Config.AzureDataFactoryName, runResponse.RunId, filterParams);

        if (pipelineRun.Status != "Succeeded")
            logger.LogError(queryResponse.Value.First().Error.ToString());

        else
            logger.LogInfo(queryResponse.Value.First().Output.ToString());

        await entityClient.SignalEntityAsync(currentJobId, "add", new RunInformation
        {
            JobId = runResponse.RunId,
            Status = pipelineRun.Status
        });
    }
}