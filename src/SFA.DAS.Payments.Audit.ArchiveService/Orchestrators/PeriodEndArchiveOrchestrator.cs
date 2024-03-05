using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Activities;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService.Orchestrators
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class PeriodEndArchiveOrchestrator
    {
        [FunctionName("PeriodEndArchiveOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Inject] IPaymentLogger log)
        {
            var messageJson = context.GetInput<string>() ??
                              throw new Exception(
                                  "Error in PeriodEndArchiveOrchestrator. Message is null.");
            try
            {
                log.LogInfo("Starting Period End Archive Orchestrator");

                await context.CallActivityAsync(nameof(StartPeriodEndArchiveActivity), messageJson);
                await context.CallSubOrchestratorAsync(nameof(ArchiveStatusOrchestrator), null, messageJson);
            }
            catch (Exception ex)
            {
                await context.CallActivityAsync<string>(nameof(ArchiveFailActivity), messageJson);
                throw new Exception("Error in PeriodEndArchiveOrchestrator", ex);
            }
        }
    }
}