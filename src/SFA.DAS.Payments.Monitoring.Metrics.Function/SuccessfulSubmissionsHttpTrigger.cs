using System.Threading.Tasks;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public static class SuccessfulSubmissionsHttpTrigger
    {
        [FunctionName("SuccessfulSubmission")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/Submission/Successful")] HttpRequest req,
            [Inject] ISubmissionJobsService submissionService)
        {
            short.TryParse(req.Query["academicYear"], out var academicYear);
            byte.TryParse(req.Query["collectionPeriod"], out var collectionPeriod);
                
            var results = await submissionService.SuccessfulSubmissionsForCollectionPeriod(academicYear, collectionPeriod);
            return new OkObjectResult(results);
        }
    }
}
