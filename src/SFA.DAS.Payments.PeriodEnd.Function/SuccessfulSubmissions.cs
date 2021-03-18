using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.PeriodEnd.Application.Services;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.Function
{
    public static class SuccessfulSubmissions
    {
        [FunctionName("SuccessfulSubmissions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/PeriodEnd/SuccessfulSubmissions")] HttpRequest req,
            [Inject] IProvidersRequiringReprocessingService providersRequiringReprocessingService,
            [Inject] IPaymentLogger logger)
        {
            if (!short.TryParse(req.Query["academicYear"], out var academicYear) || 
                !byte.TryParse(req.Query["collectionPeriod"], out var collectionPeriod))
                return new StatusCodeResult(400);
            
            logger.LogDebug($"Entering {nameof(SuccessfulSubmissions)} Function for AcademicYear: {academicYear} and Collection Period: {collectionPeriod}");

            var submissionJobs = await providersRequiringReprocessingService.SuccessfulSubmissions(academicYear, collectionPeriod);

            logger.LogInfo("Successfully retrieved latest successful submission jobs");

            return new OkObjectResult(submissionJobs);
        }
    }
}