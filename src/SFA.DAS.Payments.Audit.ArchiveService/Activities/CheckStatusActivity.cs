using System;
using System.Linq;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class CheckStatusActivity
    {
        [FunctionName(nameof(CheckStatusActivity))]
        public static async Task<StatusHelper.ArchiveStatus> Run([ActivityTrigger] string messageJson,
            [DurableClient] IDurableEntityClient entityClient,
            [Inject] IPaymentLogger logger,
            [Inject] IPeriodEndArchiveConfiguration config)
        {
            var currentRunInfo = await StatusHelper.GetCurrentJobs(entityClient);
            try
            {
                var client = await DataFactoryHelper.CreateClient(config);

                var pipelineRun = await client.PipelineRuns.GetAsync(
                    config.ResourceGroup, config.AzureDataFactoryName, currentRunInfo.InstanceId);

                logger.LogInfo("Period End Archive Status: " + pipelineRun.Status);
                if (pipelineRun.Status is "InProgress" or "Queued")
                {
                    currentRunInfo = new ArchiveRunInformation
                    {
                        JobId = currentRunInfo.JobId,
                        InstanceId = currentRunInfo.InstanceId,
                        Status = pipelineRun.Status
                    };
                    await StatusHelper.UpdateCurrentJobStatus(entityClient, currentRunInfo);

                    return StatusHelper.ArchiveStatus.InProgress;
                }


                var filterParams = new RunFilterParameters(
                    DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(10));
                var queryResponse = await client.ActivityRuns.QueryByPipelineRunAsync(
                    config.ResourceGroup, config.AzureDataFactoryName, currentRunInfo.InstanceId, filterParams);

                if (pipelineRun.Status != "Succeeded")
                {
                    throw new Exception(
                        $"Error in CheckStatusActivity. Pipeline run failed. Status: {pipelineRun.Status}. Message: {messageJson}");
                }

                logger.LogInfo(queryResponse.Value.First().Output.ToString());

                currentRunInfo = new ArchiveRunInformation
                {
                    JobId = currentRunInfo.JobId,
                    InstanceId = currentRunInfo.InstanceId,
                    Status = pipelineRun.Status
                };
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentRunInfo);
                return StatusHelper.ArchiveStatus.Completed;
            }
            catch
            {
                currentRunInfo.Status = "Failed";
                await StatusHelper.UpdateCurrentJobStatus(entityClient, currentRunInfo);
                return StatusHelper.ArchiveStatus.Failed;
            }
        }
    }
}