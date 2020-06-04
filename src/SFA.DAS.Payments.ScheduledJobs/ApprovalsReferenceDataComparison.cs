using System;
using AzureFunctions.Autofac;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Processors;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class ApprovalsReferenceDataComparisonFunction
    {
        [FunctionName("ApprovalsReferenceDataComparison")]
        public static void Run([TimerTrigger("%ApprovalsReferenceDataComparisonSchedule%", RunOnStartup = true)]TimerInfo myTimer, [Inject]IApprovalsReferenceDataComparisonProcessor processor, ILogger log)
        {
            RunApprovalsReferenceDataComparison(processor, log);
        }

        private static void RunApprovalsReferenceDataComparison(IApprovalsReferenceDataComparisonProcessor processor, ILogger log)
        {
            try
            {
                processor.ProcessComparison();
            }
            catch (Exception e)
            {
                log.Log(LogLevel.Error, e, "Error in ProcessComparison");
                throw;
            }
        }
    }
}