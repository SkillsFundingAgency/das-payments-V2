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
            Message message = null;
            try
            {
                message  = context.Extensions.Get<Message>();
            }
            catch (Exception e)
            {
                logger.LogError($"Unable to retrieve message: Error: {e.Message}",e);
            }

            if (message == null)
            {
                await next();
                return;
            }

            var lockedUntil = message.SystemProperties.LockedUntilUtc;
            if (DateTime.UtcNow > lockedUntil)
            {
                var timeoutMessage = $"Message has timed out before processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(timeoutMessage);
                throw new InvalidOperationException(timeoutMessage);
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
