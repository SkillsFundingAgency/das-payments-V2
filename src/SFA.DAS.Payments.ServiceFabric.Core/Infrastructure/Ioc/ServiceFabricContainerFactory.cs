using Autofac;
using Autofac.Integration.ServiceFabric;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;

namespace SFA.DAS.Payments.ServiceFabric.Core.Infrastructure.Ioc
{
    public static class ServiceFabricContainerFactory
    {
        public static IContainer CreateContainerForActor<TActor>() where TActor : ActorBase
        {
            var builder = ContainerFactory.CreateBuilder();
            builder.RegisterActor<TActor>();
            var container = ContainerFactory.CreateContainer(builder);
            return container;
        }

        public static IContainer CreateContainerForStatefulService<TStatefulService>() where TStatefulService : StatefulService
        {
            var builder = ContainerFactory.CreateBuilder();
            builder.RegisterStatefulService<TStatefulService>(typeof(TStatefulService).Namespace + "Type");
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