using System;
using System.Threading.Tasks;
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
        public static async Task TimerTriggerApprenticeshipsReferenceDataComparison([TimerTrigger("%ApprenticeshipValidationSchedule%", RunOnStartup = false)]TimerInfo myTimer, [Inject]IApprenticeshipsDataService service, [Inject] IPaymentLogger log)
        {
            await RunApprenticeshipsReferenceDataComparison(service, log);
        }

        [FunctionName("HttpTriggerApprenticeshipsReferenceDataComparison")]
        public static async Task HttpTriggerApprenticeshipsReferenceDataComparison(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest httpRequest,
            [Inject]IApprenticeshipsDataService service, [Inject] IPaymentLogger log)
        {
            await RunApprenticeshipsReferenceDataComparison(service, log);
        }

        private static async Task RunApprenticeshipsReferenceDataComparison(IApprenticeshipsDataService service, IPaymentLogger log)
        {
            try
            {
                await service.ProcessComparison();
            }
            catch (Exception e)
            {
                log.LogError("Error in ProcessComparison", e);
                throw;
            }
        }
    }
}