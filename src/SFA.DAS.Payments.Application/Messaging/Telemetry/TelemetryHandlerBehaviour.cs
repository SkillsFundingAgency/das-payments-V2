using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Messages.Core.Events;
using System;
using System.Threading.Tasks;

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

        public override Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            var operationId = (context.MessageBeingHandled as IEvent)?.EventId.ToString();

            return TelemetryEvent.TrackAsync(
                telemetry, 
                context.MessageHandler.HandlerType.FullName, 
                operationId, 
                () => next());
        }
    }
}