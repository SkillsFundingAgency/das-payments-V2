using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules;
using SFA.DAS.Payments.Monitoring.Submissions.Functions.Infrastructure.IoC.Modules;
using ConfigurationModule = SFA.DAS.Payments.Monitoring.Submissions.Functions.Infrastructure.IoC.Modules.ConfigurationModule;
using PaymentDataContextModule = SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules.PaymentDataContextModule;

namespace SFA.DAS.Payments.Monitoring.Submissions.Functions.Infrastructure.IoC
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
            builder.RegisterModule<FunctionsModule>();
            
            builder.RegisterModule<PaymentDataContextModule>();
            builder.RegisterModule<ConfigurationModule>();
        }
    }
}
