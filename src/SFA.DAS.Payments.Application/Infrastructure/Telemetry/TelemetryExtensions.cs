using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

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

        public static void TrackDuration(this ITelemetry telemetry, string eventName, Stopwatch stopwatch, IPaymentsEvent paymentEvent, long? employerAccountId = null)
        {
            stopwatch.Stop();
            TrackDuration(telemetry, eventName, stopwatch.Elapsed, paymentEvent, employerAccountId);
        }

        public static void TrackDuration(this ITelemetry telemetry, string eventName, TimeSpan duration, IPaymentsEvent paymentEvent, long? employerAccountId = null)
        {
            var props = new Dictionary<string, string>
            {
                {TelemetryKeys.LearnerRef, paymentEvent.Learner?.ReferenceNumber},
                {TelemetryKeys.CollectionPeriod, paymentEvent.CollectionPeriod.Period.ToString()},
                {TelemetryKeys.AcademicYear, paymentEvent.CollectionPeriod.AcademicYear.ToString()},
                {"JobId", paymentEvent.JobId.ToString()},
            };

            TrackDuration(telemetry, eventName, duration, props, employerAccountId, null);
        }

        public static void TrackDuration(this ITelemetry telemetry, string eventName, Stopwatch stopwatch, IPaymentsCommand paymentCommand, long? employerAccountId = null)
        {
            stopwatch.Stop();
            TrackDuration(telemetry, eventName, stopwatch.Elapsed, paymentCommand, employerAccountId);
        }

        public static void TrackDuration(this ITelemetry telemetry, string eventName, TimeSpan duration, IPaymentsCommand paymentCommand, long? employerAccountId = null)
        {
            var props = new Dictionary<string, string>
            {
                {"JobId", paymentCommand.JobId.ToString()},
            };

            TrackDuration(telemetry, eventName, duration, props, employerAccountId, null);
        }

        public static void TrackDurationWithMetrics(this ITelemetry telemetry, string eventName, Stopwatch stopwatch, IPaymentsCommand paymentCommand, long? employerAccountId = null, Dictionary<string, double> metrics = null)
        {
            stopwatch.Stop();
            var props = new Dictionary<string, string>
            {
                { "JobId", paymentCommand.JobId.ToString() },
            };
            TrackDuration(telemetry, eventName, stopwatch.Elapsed, props, employerAccountId, null);
        }

        private static void TrackDuration(ITelemetry telemetry, string eventName, TimeSpan duration, Dictionary<string, string> props, long? employerAccountId = null, Dictionary<string, double> metrics = null)
        {
            if (employerAccountId.HasValue)
                props.Add("Employer", employerAccountId.ToString());

            if (metrics != null)
            {
                metrics.Add(TelemetryKeys.Duration, duration.TotalMilliseconds);
            }
            else
            {
                metrics = new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, duration.TotalMilliseconds }
                };
            }

            telemetry.TrackEvent(eventName, props, metrics);
        }
    }
}