using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class ExtendLockDurationBehaviour : Behavior<ITransportReceiveContext>
    {
        private readonly IMessageReceiver messageReceiver;
        private readonly IPaymentLogger logger;

        public ExtendLockDurationBehaviour(IMessageReceiver messageReceiver, IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
        }

        public override async Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            Message message;
            try
            {
                message = context.Extensions.Get<Message>();
            }
            catch (KeyNotFoundException)
            {
                logger.LogWarning("Error getting Azure service bus Message from IIncomingLogicalMessageContext");
                await next().ConfigureAwait(false);
                return;
            }
            
            var lockedUntilUtc = message.SystemProperties.LockedUntilUtc;

            //logger.LogVerbose($"ExtendLockDurationBehaviour: current time: {DateTime.UtcNow:G}, type: {message.UserProperties["NServiceBus.EnclosedMessageTypes"]}");

            if (lockedUntilUtc <= DateTime.UtcNow.AddMinutes(1))
            {
                logger.LogInfo($"Renewing lock. Token: {message.SystemProperties.LockToken}, currently locked until: {lockedUntilUtc:G}");

                await messageReceiver.RenewLockAsync(message).ConfigureAwait(false);

                var renewedLockTime = message.SystemProperties.LockedUntilUtc;

                logger.LogInfo($"Renewed lock time: {renewedLockTime:G}, current time: {DateTime.UtcNow:G}");
            }

            await next().ConfigureAwait(false);
        }
    }
}