using System;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.ApprenticeshipsReferenceDataComparison
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class ApprenticeshipsReferenceDataComparisonTrigger
    {
        [FunctionName("TimerTriggerApprenticeshipsReferenceDataComparison")]
        public static void TimerTriggerApprenticeshipsReferenceDataComparison([TimerTrigger("%ApprenticeshipsReferenceDataComparisonSchedule%", RunOnStartup = true)]TimerInfo myTimer, [Inject]IApprenticeshipsReferenceDataComparisonService service, [Inject] ILogger log)
        {
            RunApprenticeshipsReferenceDataComparison(service, log);
        }

        [FunctionName("HttpTriggerApprenticeshipsReferenceDataComparison")]
        public static void HttpTriggerApprenticeshipsReferenceDataComparison(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject]IApprenticeshipsReferenceDataComparisonService service, [Inject] ILogger log)
        {
            RunApprenticeshipsReferenceDataComparison(service, log);
        }

        private static void RunApprenticeshipsReferenceDataComparison(IApprenticeshipsReferenceDataComparisonService service, ILogger log)
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