using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public class EstimateSubmissionWindowMetricsTimerTrigger
    {
        [FunctionName("EstimateSubmissionWindowMetrics")]
        public static async Task RunOnTimer(
            [TimerTrigger("%EstimateSubmissionWindowMetricsSchedule%", RunOnStartup=false)]TimerInfo myTimer,
            [Inject] ISubmissionWindowValidationService submissionWindowValidationService)
        {
            await submissionWindowValidationService.EstimateSubmissionWindowMetrics();
        }
    }
}