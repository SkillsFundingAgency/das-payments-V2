using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.Infrastructure.IoC
{
    public class DependencyInjectionConfig
    {
        public DependencyInjectionConfig(string functionName)
        {
            DependencyInjection.Initialize(RegisterModules, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<TelemetryModule>();
            builder.RegisterModule<LoggingModule>();
            
            builder.RegisterModule<Modules.ConfigurationModule>();
            builder.RegisterModule<Modules.FunctionsModule>();
        }
    }
}
