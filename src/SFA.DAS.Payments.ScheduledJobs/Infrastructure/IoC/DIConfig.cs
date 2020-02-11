using Autofac;
using AzureFunctions.Autofac.Configuration;
using SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC.Modules;

namespace SFA.DAS.Payments.ScheduledJobs.Infrastructure.IoC
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                RegisterModules(builder);

            }, functionName);
        }

        private static void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<MessagingModule>();
        }
    }
}
