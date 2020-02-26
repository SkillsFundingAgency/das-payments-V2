using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class DoNotProcessLockLostMessagesBehavior : Behavior<ITransportReceiveContext>
    {
        private readonly IPaymentLogger logger;

        public DoNotProcessLockLostMessagesBehavior(IPaymentLogger logger)
        {
            this.logger = logger;
        }
        
        public override Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            var message = context.Extensions.Get<Message>();

            var lockedUntilUtc = message.SystemProperties.LockedUntilUtc;
            
            if (lockedUntilUtc > DateTime.UtcNow) return next();
            
            logger.LogWarning($"Message Lock Expired: current time: {DateTime.UtcNow:G}, type: {message.UserProperties["NServiceBus.EnclosedMessageTypes"]}");

            context.AbortReceiveOperation();

            return Task.CompletedTask;
        }
    }
}