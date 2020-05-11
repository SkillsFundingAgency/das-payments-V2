using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;
// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class EarningAuditDataCleanUp
    {
        [FunctionName("EarningEventAuditDataCleanUp")]
        public static async Task EarningEventAuditDataCleanUp([ServiceBusTrigger("%EarningAuditDataCleanUpQueue%", Connection = "ServiceBusConnectionString")] string message,
                                                              [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            var batch = JsonConvert.DeserializeObject<SubmissionJobsToBeDeletedBatch>(message);
            
            await auditDataCleanUpService.EarningEventAuditDataCleanUp(batch);
        }
    }
}