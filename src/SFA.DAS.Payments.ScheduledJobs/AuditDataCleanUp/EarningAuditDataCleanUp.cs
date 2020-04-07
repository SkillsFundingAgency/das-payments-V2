using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class EarningAuditDataCleanUp
    {
        [FunctionName("EarningEventAuditDataCleanUp")]
        public static async Task EarningEventAuditDataCleanUp([TimerTrigger("%AuditDataCleanUpSchedule%", RunOnStartup = false)] TimerInfo myTimer,
                                                              [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.EarningEventAuditDataCleanUp();
        }

        [FunctionName("HttpEarningEventAuditDataCleanUp")]
        public static async Task HttpEarningEventAuditDataCleanUp([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, 
                                                                  [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.EarningEventAuditDataCleanUp();
        }
    }
}