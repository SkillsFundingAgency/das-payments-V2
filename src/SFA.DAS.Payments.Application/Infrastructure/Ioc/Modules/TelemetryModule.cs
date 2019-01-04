using Autofac;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using SFA.DAS.Payments.Application.Infrastructure.Telemetry;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class TelemetryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper>();
                    return new TelemetryConfiguration(configHelper.GetSetting("ApplicationInsightsInstrumentationKey"));
                })
                .As<TelemetryConfiguration>();

            builder.Register((c, p) =>
                {
                    var config = c.Resolve<TelemetryConfiguration>();
                    return new TelemetryClient(config) { InstrumentationKey = config.InstrumentationKey };
                })
                .As<TelemetryClient>()
                .SingleInstance();

            builder.RegisterType<ApplicationInsightsTelemetry>()
                .As<ITelemetry>()
                .InstancePerLifetimeScope();
        }
    }
}