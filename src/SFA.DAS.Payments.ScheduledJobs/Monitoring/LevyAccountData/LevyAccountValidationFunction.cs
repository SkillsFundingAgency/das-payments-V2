using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public class LevyAccountValidationFunction
    {
        [FunctionName("TimerTriggerLevyAccountValidation")]
        public static async Task TimerTriggerLevyAccountValidation(
            [TimerTrigger("%LevyAccountValidationSchedule%", RunOnStartup = false)] TimerInfo timerInfo,
            [Inject] ILevyAccountValidationService levyAccountValidationService)
        {
            await levyAccountValidationService.Validate();
        }

        [FunctionName("HttpTriggerLevyAccountValidation")]
        public static async Task HttpTriggerLevyAccountValidation(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject] ILevyAccountValidationService levyAccountValidationService)
        {
            await levyAccountValidationService.Validate();
        }
    }
}
