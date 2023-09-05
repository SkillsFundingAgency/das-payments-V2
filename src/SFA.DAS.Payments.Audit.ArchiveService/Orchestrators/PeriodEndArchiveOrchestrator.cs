using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Activities;
using SFA.DAS.Payments.Audit.ArchiveService.Infrastructure.IoC;

namespace SFA.DAS.Payments.Audit.ArchiveService.Orchestrators;

[DependencyInjectionConfig(typeof(DependencyRegister))]
public static class PeriodEndArchiveOrchestrator
{
    [FunctionName("PeriodEndArchiveOrchestrator")]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        [DurableClient] IDurableEntityClient client,
        [Inject] IPaymentLogger logger)
    {
        return await context.CallActivityAsync<string>(nameof(StartPeriodEndArchiveActivity), logger);
    }
}