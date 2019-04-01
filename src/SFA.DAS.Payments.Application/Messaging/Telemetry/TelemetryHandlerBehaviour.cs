using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Application.Messaging.Telemetry
{
    public class TelemetryHandlerBehaviour :
        Behavior<IInvokeHandlerContext>
    {
        private readonly ITelemetry telemetry;

        public TelemetryHandlerBehaviour(ITelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            using (var operation = telemetry.StartOperation())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var failure = string.Empty;
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    failure = ex.Message;
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                    telemetry.TrackEvent(context.MessageHandler.HandlerType.FullName,
                        !string.IsNullOrEmpty(failure)
                            ? new Dictionary<string, string> {{"Failed", "True"}, {"Failure", failure}}
                            : new Dictionary<string, string> {{"Failed", "False"}},
                        new Dictionary<string, double> {{TelemetryKeys.Duration, stopwatch.ElapsedMilliseconds}});
                    telemetry.StopOperation(operation);
                }
            }
        }
    }
}