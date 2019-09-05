using System;
using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using NServiceBus.UnitOfWork;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.ServiceFabric.Core.Batch;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc
{
    public static class ServiceFabricContainerFactory
    {
        public static IContainer CreateContainerForActor<TActor>(int idleTimeInSeconds = 300,int scanIntervalInSeconds = 30) where TActor : Actor
        {

            var builder = ContainerFactory.CreateBuilder();
            builder.RegisterActor<TActor>(settings: new ActorServiceSettings()
                {
                    ActorGarbageCollectionSettings =
                        new ActorGarbageCollectionSettings(idleTimeInSeconds, scanIntervalInSeconds)
                })
                .OnActivating(e =>
                {
                    ((ActorStateManagerProvider) e.Context.Resolve<IActorStateManagerProvider>()).Current = e.Instance.StateManager;
                    ((ActorIdProvider) e.Context.Resolve<IActorIdProvider>()).Current = e.Instance.Id;
                });
            var container = ContainerFactory.CreateContainer(builder);
            return container;
        }

        public static IContainer CreateContainerForStatefulService<TStatefulService>() where TStatefulService : StatefulService
        {
            var builder = ContainerFactory.CreateBuilder();

            builder.RegisterType<StateManagerUnitOfWork>().As<IManageUnitsOfWork>().InstancePerLifetimeScope();

            builder.RegisterStatefulService<TStatefulService>(typeof(TStatefulService).Namespace + "Type")
                .OnActivating(e =>
                {
                    ((ReliableStateManagerProvider) e.Context.Resolve<IReliableStateManagerProvider>()).Current = e.Instance.StateManager;
                });
            var container = ContainerFactory.CreateContainer(builder);
            var endpointConfiguration = container.Resolve<EndpointConfiguration>();
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations =>
            {
                customizations.ExistingLifetimeScope(container);
            });
            return container;
        }

        public static ContainerBuilder CreateBuilderForStatelessService<TStatelessService>() where TStatelessService : StatelessService
        {
            return CreateBuilderForStatelessService<TStatelessService>(typeof(TStatelessService).Namespace + "Type");
        }

        public static IContainer CreateContainerForStatelessService<TStatelessService>() where TStatelessService : StatelessService
        {
            var builder = CreateBuilderForStatelessService<TStatelessService>();
            var container = ContainerFactory.CreateContainer(builder);
            var endpointConfiguration = container.Resolve<EndpointConfiguration>();
            endpointConfiguration.UseContainer<AutofacBuilder>(customizations =>
            {
                customizations.ExistingLifetimeScope(container);
            });
            return container;
        }

        public static ContainerBuilder CreateBuilderForStatelessService<TStatelessService>(string serviceTypeName) where TStatelessService : StatelessService
        {
            var builder = ContainerFactory.CreateBuilder();
            builder.RegisterStatelessService<TStatelessService>(serviceTypeName);
            return builder;
        }
    }
}