using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.Application.Infrastructure.Telemetry
{
    public static class TelemetryExtensions
    {
        public static void TrackDependency(this ITelemetry telemetry, DependencyType dependencyType, string dependencyName, DateTimeOffset startTime,
            TimeSpan duration, bool success,
            Dictionary<string, string> properties = null)
        {
            telemetry.TrackDependency(dependencyType.ToString("G"), dependencyName, startTime, duration, success);
        }

        public static void AddContextInfo(this ITelemetry telemetry, long jobId)
        {
            telemetry.AddProperty("Job Id", jobId.ToString());
        }

        public static void AddContextInfo(this ITelemetry telemetry, long jobId, long ukprn)
        {
            AddContextInfo(telemetry, jobId);
            telemetry.AddProperty("Ukprn", ukprn.ToString());
        }
        public static void AddContextInfo(this ITelemetry telemetry, long jobId, long ukprn, string learnerReference)
        {
            AddContextInfo(telemetry, jobId, ukprn);
            telemetry.AddProperty("Learner Reference", learnerReference);
        }
    }
}