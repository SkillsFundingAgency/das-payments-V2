using System;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

namespace SFA.DAS.Payments.Application.Infrastructure.Logging
{
    public class PaymentsLoggerConfigurationBuilder : LoggerConfigurationBuilder, ILoggerConfigurationBuilder
    {
        private readonly TelemetryConfiguration telemetryConfig;

        public PaymentsLoggerConfigurationBuilder(TelemetryConfiguration config)
        {
            this.telemetryConfig = config ?? throw new ArgumentNullException(nameof(config));
        }

        public new LoggerConfiguration Build(IApplicationLoggerSettings applicationLoggerSettings)
        {
            var config = base.Build(applicationLoggerSettings);
            config.WriteTo.ApplicationInsightsTraces(telemetryConfig.InstrumentationKey);
            return config;
        }

    }
}