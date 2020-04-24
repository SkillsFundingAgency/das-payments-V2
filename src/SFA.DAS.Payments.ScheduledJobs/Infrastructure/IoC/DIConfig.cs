using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(RegisterModules, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<TelemetryModule>();
            builder.RegisterModule<LoggingModule>();

            builder.RegisterModule<FunctionsModule>();
            builder.RegisterModule<Modules.ConfigurationModule>();
            builder.RegisterModule<Modules.MessagingModule>();
        }
    }
}
