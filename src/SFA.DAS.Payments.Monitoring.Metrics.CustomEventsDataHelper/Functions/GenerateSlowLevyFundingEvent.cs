using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Functions
{
    [DependencyInjectionConfig(typeof(DependencyRegister))]
    public class GenerateSlowLevyFundingEvent
    {
        [FunctionName("GenerateSlowLevyFundingEvent")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            [Inject] ITelemetry telemetry)
        {
            var eventName = "Finished Job";

            Dictionary<string, string> properties = Helpers.ExtractGenericProperties(req);
            AugmentPropertiesWithJobEventParameters(req, properties);

            var metrics = new Dictionary<string, double>
            {
                { TelemetryKeys.Duration, 100000},
            };

            telemetry.TrackEvent(eventName, properties, metrics);

            var responseMessage = $"'{eventName}' event artificially triggered by SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper";

            log.LogInformation(responseMessage);

            return new OkObjectResult(responseMessage);
        }

        private static void AugmentPropertiesWithJobEventParameters(HttpRequest req, Dictionary<string, string> properties)
        {
            var jobType = Helpers.ExtractQueryParameterOrDefault(req, "JobType", "JobType");
            properties.Add("JobType", jobType);

            var status = Helpers.ExtractQueryParameterOrDefault(req, "Status", "Status");
            properties.Add("Status", status);

            properties.Add("ProcessName", "SFA.DAS.Payments.FundingSource.LevyFundedProxyService");
        }
    }
}
