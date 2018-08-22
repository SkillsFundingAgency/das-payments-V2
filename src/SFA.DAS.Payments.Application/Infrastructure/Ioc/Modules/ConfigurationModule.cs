using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Ioc.Configuration;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.Application.Infrastructure.Ioc.Modules
{
    public class ConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((c, p) =>
                {
                    var configHelper = c.Resolve<IConfigurationHelper >();
                    return new ApplicationConfiguration
                    {
                        EndpointName = configHelper.GetSetting("EndpointName"),
                        StorageConnectionString = configHelper.GetConnectionString("StorageConnectionString"),
                        ServiceBusConnectionString = configHelper.GetConnectionString("ServiceBusConnectionString")
                    };
                })
                .As<IApplicationConfiguration>()
                .SingleInstance();
        }
    }
}