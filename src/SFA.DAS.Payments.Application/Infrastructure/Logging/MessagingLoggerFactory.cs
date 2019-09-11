using System;
using NServiceBus.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    internal class MessagingLoggerFactory : ILoggerFactory
    {
        private readonly IPaymentLogger logger;

        public MessagingLoggerFactory(IPaymentLogger logger)
        {
            this.logger = logger;
        }

        public ILog GetLogger(Type type)
        {
            return new MessagingLogger(logger);
        }

        public ILog GetLogger(string name)
        {
            return new MessagingLogger(logger);
        }
    }
}