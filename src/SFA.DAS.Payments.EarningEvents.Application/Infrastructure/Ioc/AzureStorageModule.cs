using Autofac;
using ESFA.DC.FileService;
using ESFA.DC.FileService.Config;
using ESFA.DC.FileService.Config.Interface;
using ESFA.DC.FileService.Interface;
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

                return new AzureStorageFileServiceConfiguration
                {
                    ConnectionString =
                        configHelper.GetConnectionString("DcStorageConnectionString")
                };

            }).As<IAzureStorageFileServiceConfiguration>().SingleInstance();

            builder.RegisterType<AzureStorageFileService>()
                .As<IFileService>().InstancePerLifetimeScope();
        }
    }
}