using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public class EndpointCommunicationSender<T> : EndpointCommunicationBase<T>, IEndpointCommunicationSender<T> where T : IPaymentsMessage
    {
        private readonly string _destinationEndpointName;

        public EndpointCommunicationSender(string endpointName, string storageConnectionString, string destinationEndpointName, ILifetimeScope lifetimeScope) 
            : base(endpointName, storageConnectionString, lifetimeScope)
        {
            _destinationEndpointName = destinationEndpointName;
        }

        protected override void OnConfigure(EndpointConfiguration configuration, TransportExtensions<AzureStorageQueueTransport> transport)
        {
            transport.Routing().RouteToEndpoint(typeof(T).Assembly, _destinationEndpointName);
            configuration.SendOnly();
        }

        public async Task Send(T message)
        {
            if (EndpointInstance == null)
                await StartEndpoint().ConfigureAwait(false);

            await EndpointInstance.Send(message).ConfigureAwait(false);
        }
    }
}