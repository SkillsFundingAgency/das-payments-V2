using System;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class ApprovalsReferenceDataComparisonTrigger
    {
        [FunctionName("ApprovalsReferenceDataComparison")]
        public static void Run([TimerTrigger("%ApprovalsReferenceDataComparisonSchedule%", RunOnStartup = true)]TimerInfo myTimer, [Inject]IApprovalsReferenceDataComparisonService service, ILogger log)
        {
            RunApprovalsReferenceDataComparison(service, log);
        }

        private static void RunApprovalsReferenceDataComparison(IApprovalsReferenceDataComparisonService service, ILogger log)
        {
            try
            {
                service.ProcessComparison();
            }
            catch (Exception e)
            {
                log.Log(LogLevel.Error, e, "Error in ProcessComparison");
                throw;
            }
        }
    }
}