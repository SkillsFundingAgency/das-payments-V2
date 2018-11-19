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
                    var config = new TelemetryConfiguration(configHelper.GetSetting("ApplicationInsightsInstrumentationKey"));
                    return new TelemetryClient(config) {InstrumentationKey = config.InstrumentationKey};
                })
                .As<TelemetryClient>()
                .SingleInstance();

            builder.RegisterType<ApplicationInsightsTelemetry>()
                .As<ITelemetry>()
                .InstancePerLifetimeScope();
        }
    }
}