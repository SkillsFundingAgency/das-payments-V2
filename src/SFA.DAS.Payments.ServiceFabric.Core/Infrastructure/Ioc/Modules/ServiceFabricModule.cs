using System.Fabric;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Client;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc.Modules
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorProxyFactory>().As<IActorProxyFactory>();
            builder.RegisterServiceFabricSupport();
            builder.RegisterType<ServiceFabricConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
            builder.RegisterType<ReliableStateManagerProvider>().As<IReliableStateManagerProvider>().SingleInstance();
        }
    }
}