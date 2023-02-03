using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Functions
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public class GenerateFinishedEASProcessingEvent
    {
        [FunctionName("GenerateFinishedEASProcessingEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ITelemetry telemetry)
        {
            Dictionary<string, string> properties = Helpers.ExtractGenericProperties(req);

            var isSuccessful = Helpers.ExtractQueryParameterOrDefault(req, "isSuccessful", "false");

            properties.Add("isSuccessful", isSuccessful);

            var metrics = new Dictionary<string, double>
            {
                { TelemetryKeys.Duration, 10000},
            };

            var eventName = "Finished processing EAS";

            telemetry.TrackEvent(eventName, properties, metrics);

            var responseMessage = $"'{eventName}' event artificially triggered by SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper";

            log.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }
    }
}