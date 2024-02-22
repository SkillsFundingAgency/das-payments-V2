using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Extensions;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class StartPeriodEndArchiveActivity
    {
        public static async Task<DataFactoryManagementClient> CreateClient(IPeriodEndArchiveConfiguration config)
        {
            // Authenticate and create a data factory management client
            var app = ConfidentialClientApplicationBuilder.Create(config.ApplicationId)
                .WithAuthority(config.AuthorityUri + config.TenantId)
                .WithClientSecret(config.AuthenticationKey)
                .WithLegacyCacheCompatibility(false)
                .WithCacheOptions(CacheOptions.EnableSharedCacheOptions)
                .Build();

            var result = await app.AcquireTokenForClient(
                    new[] { config.ManagementUri })
                .ExecuteAsync();
            var cred = new TokenCredentials(result.AccessToken);

            return new DataFactoryManagementClient(cred)
            {
                SubscriptionId = config.SubscriptionId
            };
        }

        [FunctionName(nameof(StartPeriodEndArchiveActivity))]
        public static async Task Run([ActivityTrigger] string messageJson,
            [DurableClient] IDurableEntityClient entityClient,
            [Inject] IPaymentLogger logger,
            [Inject] IPeriodEndArchiveConfiguration config)
        {
            var currentRunInfo = await StatusHelper.GetCurrentJobs(entityClient);
            var currentJobId =
                new EntityId(nameof(HandleCurrentJobId.Handle), HandleCurrentJobId.PeriodEndArchiveEntityName);
            try
            {
                var message = JsonConvert.DeserializeObject<RecordPeriodEndFcsHandOverCompleteJob>(messageJson) ??
                              throw new Exception(
                                  $"Error in StartPeriodEndArchiveActivity. Message is null. Message: {messageJson}");

                if (message.CollectionPeriod is 0 || message.CollectionYear is 0)
                    throw new Exception(
                        $"Error in StartPeriodEndArchiveActivity. CollectionPeriod or CollectionYear is invalid. CollectionPeriod: {message.CollectionPeriod}. CollectionYear: {message.CollectionYear}");

                logger.LogInfo("Starting Period End Archive Activity");


                var client = await CreateClient(config);

                // Create a pipeline run
                logger.LogInfo("Creating pipeline run...");

                var parameters = new Dictionary<string, object>
                {
                    { "CollectionPeriod", message.CollectionPeriod },
                    { "AcademicYear", message.CollectionYear }
                };

                var runResponse = client.Pipelines.CreateRunWithHttpMessagesAsync(
                    config.ResourceGroup, config.AzureDataFactoryName, config.PipeLine, parameters: parameters
                ).Result.Body;
                logger.LogInfo("Pipeline run ID: " + runResponse.RunId);
                logger.LogInfo(
                    $"PeriodEndArchive CollectionPeriod: {message.CollectionPeriod}. AcademicYear: {message.CollectionYear}");

                currentRunInfo = new RunInformation
                {
                    JobId = runResponse.RunId,
                    Status = "Started"
                };
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentJobId, currentRunInfo);


                PipelineRun pipelineRun;
                while (true)
                {
                    pipelineRun = await client.PipelineRuns.GetAsync(
                        config.ResourceGroup, config.AzureDataFactoryName, runResponse.RunId);

                    logger.LogInfo("Period End Archive Status: " + pipelineRun.Status);
                    if (pipelineRun.Status is "InProgress" or "Queued")
                    {
                        currentRunInfo = new RunInformation
                        {
                            JobId = runResponse.RunId,
                            Status = pipelineRun.Status
                        };
                        await StatusHelper.UpdateCurrentJobStatus(entityClient, currentJobId, currentRunInfo);

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
                {
                    throw new Exception(
                        $"Error in StartPeriodEndArchiveActivity. Pipeline run failed. Status: {pipelineRun.Status}. Message: {messageJson}");
                }

                logger.LogInfo(queryResponse.Value.First().Output.ToString());

                currentRunInfo = new RunInformation
                {
                    JobId = runResponse.RunId,
                    Status = pipelineRun.Status
                };
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentJobId, currentRunInfo);
            }
            catch (Exception ex)
            {
                currentRunInfo.Status = "Failed";
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentJobId, currentRunInfo);
                throw new Exception(ex.Message);
            }
        }
    }
}