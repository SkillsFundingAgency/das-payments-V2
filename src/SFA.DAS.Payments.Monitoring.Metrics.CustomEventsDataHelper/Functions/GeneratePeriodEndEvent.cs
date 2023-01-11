using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Functions
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public class GeneratePeriodEndEvent
    {
        [FunctionName("GeneratePeriodEndMetrics")]
        public static async Task<IActionResult> GeneratePeriodEndMetrics(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ITelemetry telemetry)
        {
            string responseMessage = GenerateAccuracyEvent("Finished Generating Period End Metrics", req, log, telemetry);

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("GeneratePeriodEndMetricsForProvider")]
        public static async Task<IActionResult> GeneratePeriodEndMetricsForProvider(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ITelemetry telemetry)
        {
            string responseMessage = GenerateAccuracyEvent("Finished Generating Period End Metrics for Provider", req, log, telemetry);

            return new OkObjectResult(responseMessage);
        }

        private static string GenerateAccuracyEvent(string eventName, HttpRequest req, ILogger log, ITelemetry telemetry)
        {
            Dictionary<string, string> properties = Helpers.ExtractGenericProperties(req);

            var accuracyPercentage = Helpers.ExtractQueryParameterOrDefault(req, "AccuracyPercentage", "99.90");

            var stats = new Dictionary<string, double>
            {
                { "Percentage", double.Parse(accuracyPercentage) }
            };

            telemetry.TrackEvent(eventName, properties, stats);

            var responseMessage = $"'{eventName}' event artificially triggered by SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper";

            log.LogInformation(responseMessage);

            return responseMessage;
        }
    }
}