using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Pipeline;
using NServiceBus.Transport;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class MessageTimedOutTransportReceiveBehaviour : Behavior<ITransportReceiveContext>
    {
        private readonly IPaymentLogger logger;

        public MessageTimedOutTransportReceiveBehaviour(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
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
                await next().ConfigureAwait(false);
                return;
            }

            var lockedUntil = message.SystemProperties.LockedUntilUtc;
            if (DateTime.UtcNow > lockedUntil)
            {
                var timeoutMessage = $"ITransportReceiveContext Message has timed out before processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(timeoutMessage);
                context.AbortReceiveOperation();
                return;
            }

            await next().ConfigureAwait(false);

            if (DateTime.UtcNow > lockedUntil)
            {
                var lockTimeoutMessage = $"ITransportReceiveContext Message has timed out after processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(lockTimeoutMessage);
                context.AbortReceiveOperation();
            }

        }

    }

    public class MessageTimedOutIIncomingPhysicalMessageContextBehaviour : Behavior<IIncomingPhysicalMessageContext>
    {
        private readonly IPaymentLogger logger;

        public MessageTimedOutIIncomingPhysicalMessageContextBehaviour(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            Message message = null;
            try
            {
                message = context.Extensions.Get<Message>();
            }
            catch (Exception e)
            {
                logger.LogError($"Unable to retrieve message: Error: {e.Message}", e);
            }

            if (message == null)
            {
                await next().ConfigureAwait(false);
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

    public class MessageTimedOutIIncomingLogicalMessageContextBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        private readonly IPaymentLogger logger;

        public MessageTimedOutIIncomingLogicalMessageContextBehaviour(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            Message message = null;
            try
            {
                message = context.Extensions.Get<Message>();
            }
            catch (Exception e)
            {
                logger.LogError($"Unable to retrieve message: Error: {e.Message}", e);
            }

            if (message == null)
            {
                await next().ConfigureAwait(false);
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

    public class MessageTimedOutInvokeHandlerBehaviour : Behavior<IInvokeHandlerContext>
    {
        private readonly IPaymentLogger logger;

        public MessageTimedOutInvokeHandlerBehaviour(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            Message message = null;
            try
            {
                message = context.Extensions.Get<Message>();
            }
            catch (Exception e)
            {
                logger.LogError($"Unable to retrieve message: Error: {e.Message}", e);
            }


            if (message == null)
            {
                await next().ConfigureAwait(false);
                return;
            }

            var lockedUntil = message.SystemProperties.LockedUntilUtc;
            logger.LogWarning($"Before Next {lockedUntil}");

            if (DateTime.UtcNow > lockedUntil)
            {
                var timeoutMessage = $"IInvokeHandlerContext Message has timed out before processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(timeoutMessage);
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
                return;
            }

            await next().ConfigureAwait(false);

            logger.LogWarning($"Before Next {lockedUntil}");
            if (DateTime.UtcNow > lockedUntil)
            {
                var lockTimeoutMessage = $"IInvokeHandlerContext Message has timed out after processing. Locked until: {lockedUntil}, current time: {DateTime.UtcNow} ";
                logger.LogWarning(lockTimeoutMessage);
                context.DoNotContinueDispatchingCurrentMessageToHandlers();
            }

        }


    }

}
