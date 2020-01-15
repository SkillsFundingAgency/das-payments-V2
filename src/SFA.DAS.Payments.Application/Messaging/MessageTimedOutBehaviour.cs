using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Pipeline;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class MessageTimedOutBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var lockedUntil = context.Extensions.Get<Message>().SystemProperties.LockedUntilUtc;
            if (DateTime.UtcNow > lockedUntil)
                throw new InvalidOperationException($"Message has timed out before processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ");
            await next().ConfigureAwait(false);
            if (DateTime.UtcNow > lockedUntil)
                throw new InvalidOperationException($"Message has timed out after processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ");
        }
    }
}
