using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
namespace SFA.DAS.Payments.PeriodEnd.Function.Infrastructure.IoC
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
            builder.RegisterModule<LoggingModule>();
            
            builder.RegisterModule<Modules.ConfigurationModule>();
            builder.RegisterModule<Modules.FunctionsModule>();
        }
    }
}
