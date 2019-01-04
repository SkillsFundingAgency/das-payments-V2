using System;
using ESFA.DC.Logging.Config.Interfaces;
using ESFA.DC.Logging.Interfaces;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public class PaymentsSerilogLoggerFactory : ISerilogLoggerFactory
    {
        private readonly ILoggerConfigurationBuilder loggerConfigurationBuilder;

        public PaymentsSerilogLoggerFactory(ILoggerConfigurationBuilder loggerConfigurationBuilder)
        {
            this.loggerConfigurationBuilder = loggerConfigurationBuilder ?? throw new ArgumentNullException(nameof(loggerConfigurationBuilder));
        }

        public Serilog.ILogger Build(IApplicationLoggerSettings applicationLoggerSettings)
        {
            return loggerConfigurationBuilder
                .Build(applicationLoggerSettings)
                .CreateLogger();
        }
    }
}