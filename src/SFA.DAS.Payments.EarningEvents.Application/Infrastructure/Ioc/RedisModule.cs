using System;
using Autofac;
using ESFA.DC.IO.Interfaces;
using ESFA.DC.IO.Redis;
using ESFA.DC.IO.Redis.Config;
using ESFA.DC.IO.Redis.Config.Interfaces;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.EarningEvents.Application.Infrastructure.Ioc
{
    public class RedisModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var configHelper = c.Resolve<IConfigurationHelper>();

                return new RedisKeyValuePersistenceServiceConfig()
                {
                    ConnectionString = configHelper.GetConnectionString("AzureRedisConnectionString"),
                    KeyExpiry = new TimeSpan(14, 0, 0, 0)
                };
            }).As<IRedisKeyValuePersistenceServiceConfig>().SingleInstance();

            builder.RegisterType<RedisKeyValuePersistenceService>().As<IKeyValuePersistenceService>().InstancePerLifetimeScope();
        }
    }
}