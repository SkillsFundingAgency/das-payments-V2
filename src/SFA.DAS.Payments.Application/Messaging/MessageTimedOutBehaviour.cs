using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class MessageTimedOutBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly IPaymentLogger logger;

        public MessageTimedOutBehaviour(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var lockedUntil = context.Extensions.Get<Message>().SystemProperties.LockedUntilUtc;
            if (DateTime.UtcNow > lockedUntil)
            {
                var timeoutMesssage = $"Message has timed out before processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(timeoutMesssage);
                throw new InvalidOperationException();
            }

            await next().ConfigureAwait(false);
            if (DateTime.UtcNow > lockedUntil)
            {
                var lockTimeoutMessage = $"Message has timed out after processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(lockTimeoutMessage);
                throw new InvalidOperationException(lockTimeoutMessage);
            }

        }
    }
}
