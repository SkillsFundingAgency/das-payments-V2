using System;
using ESFA.DC.Logging.Config;
using ESFA.DC.Logging.Config.Extensions;
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
            var config = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessName()
                .Enrich.WithThreadId()
                .WithMinimumLogLevel(applicationLoggerSettings.ApplicationLoggerOutputSettingsCollection)
                .Filter.ByExcluding(evnt => evnt.Exception != null && evnt.Exception.ToString().Contains("license"));
            
            config.WriteTo.ApplicationInsightsTraces(telemetryConfig.InstrumentationKey);
            return config;
        }
    }
}