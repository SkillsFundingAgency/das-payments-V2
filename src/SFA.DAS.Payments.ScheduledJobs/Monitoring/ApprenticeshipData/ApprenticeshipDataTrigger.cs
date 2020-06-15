using System;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.ApprenticeshipData
{
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class ApprenticeshipDataTrigger
    {
        [FunctionName("TimerTriggerApprenticeshipsReferenceDataComparison")]
        public static void TimerTriggerApprenticeshipsReferenceDataComparison([TimerTrigger("%ApprenticeshipValidationSchedule%", RunOnStartup = true)]TimerInfo myTimer, [Inject]IApprenticeshipsDataService service, [Inject] IPaymentLogger log)
        {
            RunApprenticeshipsReferenceDataComparison(service, log);
        }

        [FunctionName("HttpTriggerApprenticeshipsReferenceDataComparison")]
        public static void HttpTriggerApprenticeshipsReferenceDataComparison(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject]IApprenticeshipsDataService service, [Inject] IPaymentLogger log)
        {
            RunApprenticeshipsReferenceDataComparison(service, log);
        }

        private static void RunApprenticeshipsReferenceDataComparison(IApprenticeshipsDataService service, IPaymentLogger log)
        {
            try
            {
                service.ProcessComparison();
            }
            catch (Exception e)
            {
                log.LogError("Error in ProcessComparison", e);
                throw;
            }
        }
    }
}