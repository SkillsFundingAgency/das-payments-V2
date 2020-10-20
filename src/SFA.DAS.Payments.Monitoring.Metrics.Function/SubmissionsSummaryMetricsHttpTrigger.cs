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
    [DependencyInjectionConfig(typeof(DependencyInjectionConfig))]
    public static class SubmissionsSummaryMetricsHttpTrigger
    {
        [FunctionName("GetSubmissionsSummaryMetrics")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
            ISubmissionsSummaryMetricsService submissionsSummaryMetricsService)
        {
            long.TryParse(req.Query["jobId"], out var jobId);
            short.TryParse(req.Query["academicYear"], out var academicYear);
            byte.TryParse(req.Query["collectionPeriod"], out var collectionPeriod);

            var result = await submissionsSummaryMetricsService.GenrateSubmissionsSummaryMetrics(jobId, academicYear, collectionPeriod, CancellationToken.None);

            if (result == null || result.IsWithinTolerance == false)
                return new NotFoundObjectResult("");

            return new OkObjectResult(result);
        }
    }
}
