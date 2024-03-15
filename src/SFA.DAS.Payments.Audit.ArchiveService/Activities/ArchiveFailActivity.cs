using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Audit.ArchiveService.Activities
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class ArchiveFailActivity
    {
        [FunctionName(nameof(ArchiveFailActivity))]
        public static async Task Run([ActivityTrigger] string messageJson,
            [DurableClient] IDurableEntityClient entityClient,
            [Inject] IPaymentLogger logger)
        {
            var message = JsonConvert.DeserializeObject<RecordPeriodEndFcsHandOverCompleteJob>(messageJson) ??
                          throw new Exception(
                              $"Error in StartPeriodEndArchiveActivity. Message is null. Message: {messageJson}");
            var runInformation = await StatusHelper.GetCurrentJobs(entityClient);
            if(string.IsNullOrEmpty(runInformation.JobId))
            {
                runInformation.JobId = message.JobId.ToString();
            }
            runInformation.Status = "Failed";

            logger.LogError($"JobId: {runInformation.JobId}. ADF InstanceId: {runInformation.InstanceId} PeriodEndArchiveOrchestrator failed");

            await StatusHelper.UpdateCurrentJobStatus(entityClient, runInformation);
        }
    }
}