using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public abstract class EndpointCommunicationBase<T> : IDisposable where T : IPaymentsMessage
    {
        protected IEndpointInstance EndpointInstance { get; private set; }
        protected string EndpointName { get; }

        private readonly string _storageConnectionString;

        protected EndpointCommunicationBase(string endpointName, string storageConnectionString)
        {
            EndpointName = endpointName;
            _storageConnectionString = storageConnectionString;
        }

        protected async Task StartEndpoint()
        {
            var endpointConfiguration = new EndpointConfiguration(EndpointName);

            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => typeof(T).IsAssignableFrom(type));
            //conventions.DefiningEventsAs(type => typeof(T).IsAssignableFrom(type));

            var persistence = endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString(_storageConnectionString);

            endpointConfiguration.DisableFeature<TimeoutManager>();
            endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
            
            var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
            transport.ConnectionString(_storageConnectionString);
            transport.BatchSize(1);
            transport.DegreeOfReceiveParallelism(1);
            
            endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            endpointConfiguration.EnableInstallers();
            endpointConfiguration.UseContainer<AutofacBuilder>();

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
