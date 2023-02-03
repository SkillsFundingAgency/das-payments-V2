using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper.Telemetry;

namespace SFA.DAS.Payments.Monitoring.Metrics.CustomEventsDataHelper
{
    public class DependencyRegister
    {
        public DependencyRegister(string functionName)
        {
            DependencyInjection.Initialize(RegisterModules, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<TelemetryModule>();
            builder.RegisterModule<ConfigurationModule>();
        }
    }
}