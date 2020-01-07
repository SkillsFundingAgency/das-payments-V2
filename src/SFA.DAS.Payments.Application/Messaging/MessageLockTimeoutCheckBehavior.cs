using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using NServiceBus.Pipeline;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Messaging
{
    public class MessageLockTimeoutCheckBehavior : Behavior<ITransportReceiveContext>
    {
        private readonly IPaymentLogger logger;
        private readonly IApplicationConfiguration configuration;

        public MessageLockTimeoutCheckBehavior(IPaymentLogger logger, IApplicationConfiguration configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }


        public override Task Invoke(ITransportReceiveContext context, Func<Task> next)
        {
            TimeSpan configTimeoutThreshold = configuration.MessageLockTimeoutThreshold;

            var lockedUntilUtc = context.Extensions.Get<Message>().SystemProperties.LockedUntilUtc;
            lockedUntilUtc -= configTimeoutThreshold;
           
            if (lockedUntilUtc >= DateTime.UtcNow)
            {
                return next();
            }
           
            logger.LogInfo($"The given message's timeout has passed according to the configured message timeout threshold. Threshold value:{configTimeoutThreshold} ");
            context.AbortReceiveOperation();

            return Task.CompletedTask;
        }
    }
}