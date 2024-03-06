using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class StartPeriodEndArchiveActivity
    {
        [FunctionName(nameof(StartPeriodEndArchiveActivity))]
        public static async Task Run([ActivityTrigger] string messageJson,
            [DurableClient] IDurableEntityClient entityClient,
            [Inject] IPaymentLogger logger,
            [Inject] IPeriodEndArchiveConfiguration config)
        {
            var currentRunInfo = await StatusHelper.GetCurrentJobs(entityClient);

            try
            {
                var message = JsonConvert.DeserializeObject<RecordPeriodEndFcsHandOverCompleteJob>(messageJson) ??
                              throw new Exception(
                                  $"Error in StartPeriodEndArchiveActivity. Message is null. Message: {messageJson}");

                if (message.CollectionPeriod is 0 || message.CollectionYear is 0)
                    throw new Exception(
                        $"Error in StartPeriodEndArchiveActivity. CollectionPeriod or CollectionYear is invalid. CollectionPeriod: {message.CollectionPeriod}. CollectionYear: {message.CollectionYear}");

                logger.LogInfo("Starting Period End Archive Activity");


                var client = await DataFactoryHelper.CreateClient(config);

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

                currentRunInfo = new ArchiveRunInformation
                {
                    JobId = message.JobId.ToString(),
                    InstanceId = runResponse.RunId,
                    Status = "Started"
                };
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentRunInfo);
            }
            catch (Exception ex)
            {
                currentRunInfo.Status = "Failed";
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentRunInfo);
                logger.LogError( "Error in StartPeriodEndArchiveActivity", ex);
                throw;
            }
 
        }
    }
}