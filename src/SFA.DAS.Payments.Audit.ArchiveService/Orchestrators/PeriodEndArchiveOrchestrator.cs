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
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Inject] IPaymentLogger log)
        {
            try
            {
                log.LogInfo("Starting Period End Archive Orchestrator");

                var messageJson = context.GetInput<string>() ??
                                  throw new Exception(
                                      "Error in PeriodEndArchiveOrchestrator. Message is null.");
                return await context.CallActivityAsync<string>(nameof(StartPeriodEndArchiveActivity), messageJson);
            }
            catch (Exception ex)
            {
                log.LogError("Error in PeriodEndArchiveOrchestrator", ex);
                return null;
            }
        }
    }
}