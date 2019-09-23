using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Application.Messaging.Telemetry
{
    public static class TelemetryEvent
    {
        public static async Task TrackAsync(ITelemetry telemetry, string handler, string operationId, Func<Task> action)
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