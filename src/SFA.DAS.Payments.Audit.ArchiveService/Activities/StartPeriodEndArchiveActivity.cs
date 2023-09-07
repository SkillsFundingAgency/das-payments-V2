using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities;

[DependencyInjectionConfig(typeof(DependencyRegister))]
public static class StartPeriodEndArchiveActivity
{
    public static async Task<DataFactoryManagementClient> CreateClient(string applicationId, string tenantId,
        string authKey, string subscriptionId)
    {
        // Authenticate and create a data factory management client
        var app = ConfidentialClientApplicationBuilder.Create(applicationId)
            .WithAuthority("https://login.microsoftonline.com/" + tenantId)
            .WithClientSecret(authKey)
            .WithLegacyCacheCompatibility(false)
            .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
            .Build();

        var result = await app.AcquireTokenForClient(
                new[] { "https://management.azure.com//.default" })
            .ExecuteAsync();
        var cred = new TokenCredentials(result.AccessToken);

        return new DataFactoryManagementClient(cred)
        {
            SubscriptionId = subscriptionId
        };
    }

    [FunctionName(nameof(StartPeriodEndArchiveActivity))]
    public static async Task Run([ActivityTrigger] IDurableEntityClient entityClient,
        [Inject] IPaymentLogger logger,
        [Inject] IPeriodEndArchiveConfiguration config)
    {
        try
        {
            var currentJobId =
                new EntityId(nameof(HandleCurrentJobId.Handle), HandleCurrentJobId.PeriodEndArchiveEntityName);

            var client = await CreateClient(config.ApplicationId, config.TenantId, config.AuthenticationKey,
                config.SubscriptionId);

            // Create a pipeline run
            logger.LogInfo("Creating pipeline run...");

            var runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
                config.ResourceGroup, config.AzureDataFactoryName, config.PipeLine
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
                    config.ResourceGroup, config.AzureDataFactoryName, runResponse.RunId);

                logger.LogInfo("Period End Archive Status: " + pipelineRun.Status);
                if (pipelineRun.Status is "InProgress" or "Queued")
                {
                    await entityClient.SignalEntityAsync(currentJobId, "add", new RunInformation
                    {
                        JobId = runResponse.RunId,
                        Status = pipelineRun.Status
                    });
                    Thread.Sleep(config.SleepDelay);
                }
                else
                {
                    break;
                }
            }

            var filterParams = new RunFilterParameters(
                DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
            var queryResponse = await client.ActivityRuns.QueryByPipelineRunAsync(
                config.ResourceGroup, config.AzureDataFactoryName, runResponse.RunId, filterParams);

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
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
        }
    }
}