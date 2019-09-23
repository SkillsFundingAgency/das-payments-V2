using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                {TelemetryKeys.Ukprn, paymentEvent.Ukprn.ToString()},
            };
            if (employerAccountId.HasValue)
                props.Add("Employer", employerAccountId.ToString());

            telemetry.TrackEvent(eventName,
                props,
                new Dictionary<string, double>
                {
                    { TelemetryKeys.Duration, duration.TotalMilliseconds }
                });
        }

        public static void TrackAction(this ITelemetry telemetry, string handler, string operationId, Action action)
        {
            using (var operation = telemetry.StartOperation(handler, operationId))
            {
                var d = new Dictionary<string, string> { { "Failed", "False" } };
                var watch = Stopwatch.StartNew();
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    d = new Dictionary<string, string> { { "Failed", "True" }, { "Failure", ex.Message } };
                    throw;
                }
                finally
                {
                    watch.Stop();
                    telemetry.TrackEvent(handler, d, new Dictionary<string, double> { { TelemetryKeys.Duration, watch.ElapsedMilliseconds } });
                    telemetry.StopOperation(operation);
                }
            }
        }

        public static async Task TrackActionAsync(this ITelemetry telemetry, string handler, string operationId, Func<Task> action)
        {
            using (var operation = telemetry.StartOperation(handler, operationId))
            {
                var d = new Dictionary<string, string> { { "Failed", "False" } };
                var watch = Stopwatch.StartNew();
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    watch.Stop();
                    d = new Dictionary<string, string> { { "Failed", "True" }, { "Failure", ex.Message } };
                    throw;
                }
                finally
                {
                    watch.Stop();
                    telemetry.TrackEvent(handler, d, new Dictionary<string, double> { { TelemetryKeys.Duration, watch.ElapsedMilliseconds } });
                    telemetry.StopOperation(operation);
                }
            }
        }
    }
}