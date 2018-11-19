using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Application.Messaging.Telemetry
{
    public class HandlerTimingBehaviour :
        Behavior<IInvokeHandlerContext>
    {
        private readonly ITelemetry telemetry;

        public HandlerTimingBehaviour(ITelemetry telemetry)
        {
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            try
            {
                await next();
            }
            finally
            {
                stopwatch.Stop();
                telemetry.TrackDuration(context.MessageHandler.HandlerType.FullName, stopwatch.Elapsed);
            }
        }
    }
}