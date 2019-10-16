using System;
using ESFA.DC.Logging.Config.Interfaces;
using NServiceBus.Logging;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    internal class MessagingLoggerFactory : ILoggerFactory
    {
        private readonly IPaymentLogger logger;
        private readonly IApplicationLoggerSettings settings;

        public MessagingLoggerFactory(IPaymentLogger logger, IApplicationLoggerSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public ILog GetLogger(Type type)
        {
            return new MessagingLogger(logger, settings);
        }

        public ILog GetLogger(string name)
        {
            return new MessagingLogger(logger, settings);
        }
    }
}