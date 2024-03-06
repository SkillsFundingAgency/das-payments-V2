using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Activities;
using SFA.DAS.Payments.Audit.ArchiveService.Helpers;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.Configuration;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService.Orchestrators
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class PeriodEndArchiveOrchestrator
    {
        [FunctionName("PeriodEndArchiveOrchestrator")]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Inject] IPaymentLogger log,
            [Inject] IPeriodEndArchiveConfiguration config)
        {
            var messageJson = context.GetInput<string>() ??
                              throw new Exception(
                                  "Error in PeriodEndArchiveOrchestrator. Message is null.");
            try
            {
                log.LogInfo("Starting Period End Archive Orchestrator");

                await context.CallActivityAsync(nameof(StartPeriodEndArchiveActivity), messageJson);

                // Start polling ADF for result
                var timeout = context.CurrentUtcDateTime.AddMinutes(config.SleepDelay);
                var pollingInterval = TimeSpan.FromMinutes(1);


                while (context.CurrentUtcDateTime < timeout)
                {
                    var status =
                        await context.CallActivityAsync<StatusHelper.ArchiveStatus>(nameof(CheckStatusActivity),
                            messageJson);
                    if (status is StatusHelper.ArchiveStatus.Completed or StatusHelper.ArchiveStatus.Failed)
                    {
                        break;
                    }

                    // If not yet complete, or failed wait for the specified polling interval before the next attempt.
                    await context.CreateTimer(context.CurrentUtcDateTime.Add(pollingInterval), CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                await context.CallActivityAsync(nameof(ArchiveFailActivity), messageJson);
                throw new Exception("Error in PeriodEndArchiveOrchestrator", ex);
            }
        }
    }
}