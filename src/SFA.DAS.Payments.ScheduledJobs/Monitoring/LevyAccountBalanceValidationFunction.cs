using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public class LevyAccountBalanceValidationFunction
    {
        [FunctionName("TimerTriggerLevyAccountBalanceValidation")]
        public static void TimerTriggerLevyAccountBalanceValidation(
            [TimerTrigger("%LevyAccountBalanceValidationSchedule%", RunOnStartup = false)] TimerInfo timerInfo,
            [Inject] ILevyAccountBalanceValidationService levyAccountBalanceValidationService)
        {
            levyAccountBalanceValidationService.Validate();
        }

        [FunctionName("HttpTriggerLevyAccountBalanceValidation")]
        public static async Task HttpTriggerLevyAccountBalanceValidation(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject] ILevyAccountBalanceValidationService levyAccountBalanceValidationService)
        {
            await levyAccountBalanceValidationService.Validate();
        }
    }
}
