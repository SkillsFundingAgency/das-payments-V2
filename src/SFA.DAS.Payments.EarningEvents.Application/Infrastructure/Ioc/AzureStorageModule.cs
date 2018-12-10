using Autofac;
using ESFA.DC.IO.AzureStorage;
using ESFA.DC.IO.AzureStorage.Config.Interfaces;
using ESFA.DC.IO.Interfaces;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class AzureStorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();

                return new AzureStorageKeyValuePersistenceConfig(
                    configHelper.GetConnectionString("DcStorageConnectionString"),
                    configHelper.GetSetting("DcBlobStorageContainer"));
            }).As<IAzureStorageKeyValuePersistenceServiceConfig>().SingleInstance();

            builder.RegisterType<AzureStorageKeyValuePersistenceService>()
                .Keyed<IKeyValuePersistenceService>(0)
                .As<IStreamableKeyValuePersistenceService>().InstancePerLifetimeScope();
        }
    }
}
    