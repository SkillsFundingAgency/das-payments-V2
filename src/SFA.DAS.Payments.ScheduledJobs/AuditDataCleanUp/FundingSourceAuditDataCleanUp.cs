using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;
// ReSharper disable UnusedMember.Global

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class FundingSourceAuditDataCleanUp
    {
        [FunctionName("FundingSourceEventAuditDataCleanUp")]
        public static async Task FundingSourceEventAuditDataCleanUp([ServiceBusTrigger("%FundingSourceAuditDataCleanUpQueue%", Connection = "ServiceBusConnectionString")] string message,
                                                                    [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            var batch = JsonConvert.DeserializeObject<SubmissionJobsToBeDeletedBatch>(message);

            await auditDataCleanUpService.FundingSourceEventAuditDataCleanUp(batch);
        }
    }
}