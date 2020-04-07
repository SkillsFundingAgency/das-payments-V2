using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class DataLockAuditDataCleanUp
    {
        [FunctionName("DataLockEventAuditDataCleanUp")]
        public static async Task DataLockEventAuditDataCleanUp([TimerTrigger("%AuditDataCleanUpSchedule%", RunOnStartup = false)] TimerInfo myTimer,
                                                               [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.DataLockEventAuditDataCleanUp();
        }

        [FunctionName("HttpDataLockEventAuditDataCleanUp")]
        public static async Task HttpDataLockEventAuditDataCleanUp([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, 
                                                                   [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.DataLockEventAuditDataCleanUp();
        }
    }
}