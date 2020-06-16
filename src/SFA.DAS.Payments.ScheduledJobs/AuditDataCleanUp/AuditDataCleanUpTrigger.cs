using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class AuditDataCleanUpTrigger
    {
        [FunctionName("TimerTriggerAuditDataCleanup")]
        public static async Task TimerTriggerAuditDataCleanup(
            [TimerTrigger("%AuditDataCleanUpSchedule%", RunOnStartup = false)] TimerInfo timerInfo,
            [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.TriggerAuditDataCleanup();
        }

        [FunctionName("HttpTriggerAuditDataCleanup")]
        public static async Task HttpTriggerAuditDataCleanup(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.TriggerAuditDataCleanup();
        }
    }
}
