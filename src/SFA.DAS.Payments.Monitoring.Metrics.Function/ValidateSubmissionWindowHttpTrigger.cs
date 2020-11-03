using System;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class ValidateSubmissionWindowHttpTrigger
    {
        [FunctionName("ValidateSubmissionWindow")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Inject] ISubmissionWindowValidationService submissionWindowValidationService)
        {
            long.TryParse(req.Query["jobId"], out var jobId);
            short.TryParse(req.Query["academicYear"], out var academicYear);
            byte.TryParse(req.Query["collectionPeriod"], out var collectionPeriod);

            var result = await submissionWindowValidationService.ValidateSubmissionWindow(jobId, academicYear, collectionPeriod, CancellationToken.None);

            if (result == null)
                throw new ApplicationException("Error in Submission Window Validation");

            if (result.IsWithinTolerance == false) //406
                return new BadRequestObjectResult("") {StatusCode = StatusCodes.Status406NotAcceptable};

            return new OkObjectResult(result); //200
        }
    }
}
