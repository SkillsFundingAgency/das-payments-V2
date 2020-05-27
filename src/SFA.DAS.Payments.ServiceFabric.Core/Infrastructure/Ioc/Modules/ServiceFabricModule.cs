using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Cache;
using SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core.Messaging;
using EarningEventKey = SFA.DAS.Payments.ServiceFabric.Core.Messaging.EarningEventKey;

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

            builder.RegisterType<ActorReliableCollectionCache<EarningEventKey>>().AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<DuplicateEarningEventService>().As<IDuplicateEarningEventService>()
                .InstancePerLifetimeScope();
            builder.RegisterType<DuplicatePeriodisedPaymentEventService>().As<IDuplicatePeriodisedPaymentEventService>()
                .InstancePerLifetimeScope();

        }
    }
}