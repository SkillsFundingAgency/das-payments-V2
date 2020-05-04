using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class MessageTimeOutBehaviour : Behavior<ITransportReceiveContext>
    {
        private readonly IMessageReceiver messageReceiver;
        private readonly IPaymentLogger logger;
        private readonly ITelemetry telemetry;

        public MessageTimeOutBehaviour(IMessageReceiver messageReceiver, IPaymentLogger logger, ITelemetry telemetry)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
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
            if (lockedUntilUtc >= DateTime.UtcNow)
            {
                logger.LogWarning($"Message lock lost, discarding the message");
                telemetry.TrackEvent("MessageLockLost", (lockedUntilUtc - DateTime.UtcNow).TotalSeconds);
            }

            if (lockedUntilUtc <= DateTime.UtcNow.AddMinutes(1))
            {
                logger.LogDebug($"Renewing lock. Token: {message.SystemProperties.LockToken}, currently locked until: {lockedUntilUtc:G}");
                await messageReceiver.RenewLockAsync(message).ConfigureAwait(false);
                var renewedLockTime = message.SystemProperties.LockedUntilUtc;
                logger.LogInfo($"Renewed lock time: {renewedLockTime:G}, current time: {DateTime.UtcNow:G}");
                telemetry.TrackEvent("RenewedMessageLock", (DateTime.UtcNow.AddMinutes(1) - lockedUntilUtc).TotalSeconds);
            }
            await next().ConfigureAwait(false);
        }
    }
}