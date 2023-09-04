using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Audit.ArchiveService.Activities;

namespace SFA.DAS.Payments.Audit.ArchiveService.Orchestrators;

public static class PeriodEndArchiveOrchestrator
{
    [FunctionName("PeriodEndArchiveOrchestrator")]
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context,
        [DurableClient] IDurableEntityClient client,
        IPaymentLogger logger)
    {
        return await context.CallActivityAsync<string>(nameof(StartPeriodEndArchiveActivity), logger);
    }
}