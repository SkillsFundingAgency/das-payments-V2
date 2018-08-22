using System;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using NServiceBus.Features;

namespace SFA.DAS.Payments.Application.Infrastructure.Messaging
{
    public abstract class EndpointCommunicationBase : IDisposable
    {
        protected IEndpointInstance EndpointInstance { get; private set; }
        protected string EndpointName { get; }

        private readonly string _storageConnectionString;
        private readonly ILifetimeScope _lifetimeScope;

        protected EndpointCommunicationBase(string endpointName, string storageConnectionString, ILifetimeScope lifetimeScope)
        {
            EndpointName = endpointName;
            _storageConnectionString = storageConnectionString;
            _lifetimeScope = lifetimeScope;
        }

        protected async Task StartEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningMessagesAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Commands") ?? false));
            conventions.DefiningEventsAs(type => (type.Namespace?.StartsWith("SFA.DAS.Payments") ?? false) && (type.Namespace?.Contains(".Messages.Events") ?? false));

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(_storageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
            transport.ConnectionString(_storageConnectionString);
            transport.BatchSize(1);
            transport.DegreeOfReceiveParallelism(1);

            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(_lifetimeScope));

            OnConfigure(endpointConfiguration, transport);

            EndpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
        }

        protected virtual void OnConfigure(EndpointConfiguration configuration, TransportExtensions<AzureStorageQueueTransport> transport)
        {
        }

        public void Dispose()
        {
            // TODO: stop and dispose of endpoint            
        }
    }
}
