using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core.UnitOfWork;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc.Modules
{
    public class ServiceFabricModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ActorProxyFactory>().As<IActorProxyFactory>();
            builder.RegisterType<ServiceProxyFactory>().As<IServiceProxyFactory>();
            builder.RegisterServiceFabricSupport();
            builder.RegisterType<ServiceFabricConfigurationHelper>().As<IConfigurationHelper>().SingleInstance();
            builder.RegisterType<ReliableStateManagerProvider>().As<IReliableStateManagerProvider>().SingleInstance();
            builder.RegisterType<ActorStateManagerProvider>().As<IActorStateManagerProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ActorIdProvider>().As<IActorIdProvider>().InstancePerLifetimeScope();
            builder.RegisterType<ReliableStateManagerTransactionProvider>().As<IReliableStateManagerTransactionProvider>().InstancePerLifetimeScope();
        }
    }
}