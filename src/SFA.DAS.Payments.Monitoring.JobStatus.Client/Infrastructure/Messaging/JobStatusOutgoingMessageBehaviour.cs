using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.Messages.Core.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Monitoring.JobStatus.Client.Infrastructure.Messaging
{
    public class JobStatusOutgoingMessageBehaviour : Behavior<IOutgoingLogicalMessageContext>
    {
        public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            await next().ConfigureAwait(false);
            if (context.Message.Instance is IPaymentsMessage && context.Extensions.TryGet("event_ids", out List<(DateTimeOffset startTime, Guid id)> eventIds))
                eventIds.Add((DateTimeOffset.UtcNow, (context.Message.Instance as IPaymentsEvent)?.EventId ?? (context.Message.Instance as PaymentsCommand)?.CommandId ?? Guid.Empty));
        }
    }
}