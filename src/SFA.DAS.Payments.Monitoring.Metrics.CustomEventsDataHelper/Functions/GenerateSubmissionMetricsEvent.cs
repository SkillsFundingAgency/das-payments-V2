using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Functions
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public class GenerateSubmissionMetricsEvent
    {
        [FunctionName("GenerateSubmissionMetricsEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ITelemetry telemetry)
        {
            var eventName = "Finished Generating Submissions Summary Metrics";

            Dictionary<string, string> properties = Helpers.ExtractGenericProperties(req);

            var accuracyPercentage = Helpers.ExtractQueryParameterOrDefault(req, "AccuracyPercentage", "99.90");

            var stats = new Dictionary<string, double>
            {
                { "Percentage", double.Parse(accuracyPercentage) }
            };

            telemetry.TrackEvent(eventName, properties, stats);

            var responseMessage = $"'{eventName}' event artificially triggered by SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper";

            log.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }
    }
}
