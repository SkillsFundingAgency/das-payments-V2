using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper
{
    public static class Helpers
    {
        public static StringValues ExtractQueryParameterOrDefault(HttpRequest req, string parameterName, string defaultValue = null)
        {
            if (!req.Query.TryGetValue(parameterName, out var outputParamter))
            {
                outputParamter = defaultValue;
            }

            return outputParamter;
        }

        public static Dictionary<string, string> ExtractGenericProperties(HttpRequest req)
        {
            var jobId = Helpers.ExtractQueryParameterOrDefault(req, "JobId", "12345");
            var collectionPeriod = Helpers.ExtractQueryParameterOrDefault(req, "CollectionPeriod", "R05");
            var academicYear = Helpers.ExtractQueryParameterOrDefault(req, "AcademicYear", "2223");
            var ukprn = Helpers.ExtractQueryParameterOrDefault(req, "Ukprn", "4242");

            var properties = new Dictionary<string, string>
            {
                { TelemetryKeys.JobId,  jobId},
                { TelemetryKeys.CollectionPeriod, collectionPeriod},
                { TelemetryKeys.AcademicYear, academicYear},
                { TelemetryKeys.Ukprn, ukprn }
            };

            return properties;
        }
    }
}
