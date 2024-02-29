using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

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
            var runInformation = await StatusHelper.GetCurrentJobs(entityClient);
            runInformation.Status = "Failed";

            logger.LogError($"JobId: {runInformation.JobId}. PeriodEndArchiveOrchestrator failed");

            await StatusHelper.UpdateCurrentJobStatus(entityClient);
        }
    }
}