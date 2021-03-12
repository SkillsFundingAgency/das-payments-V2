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
            [Inject] ISubmissionJobsService submissionJobsService,
            [Inject] IPaymentLogger logger)
        {
            logger.LogDebug($"Entering {nameof(SuccessfulSubmissions)} Function");

            var submissionJobs = await submissionJobsService.SuccessfulSubmissions();

            logger.LogInfo("Successfully retrieved latest successful submission jobs");

            return new OkObjectResult(submissionJobs);
        }
    }
}