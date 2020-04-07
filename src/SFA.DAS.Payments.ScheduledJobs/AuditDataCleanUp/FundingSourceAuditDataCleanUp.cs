using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.AuditDataCleanUp
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class FundingSourceAuditDataCleanUp
    {
        [FunctionName("FundingSourceEventAuditDataCleanUp")]
        public static async Task FundingSourceEventAuditDataCleanUp([TimerTrigger("%AuditDataCleanUpSchedule%", RunOnStartup = false)] TimerInfo myTimer,
                                                                    [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.FundingSourceEventAuditDataCleanUp();
        }

        [FunctionName("HttpFundingSourceEventAuditDataCleanUp")]
        public static async Task HttpFundingSourceEventAuditDataCleanUp([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, 
                                                                        [Inject] IAuditDataCleanUpService auditDataCleanUpService)
        {
            await auditDataCleanUpService.FundingSourceEventAuditDataCleanUp();
        }
    }
}