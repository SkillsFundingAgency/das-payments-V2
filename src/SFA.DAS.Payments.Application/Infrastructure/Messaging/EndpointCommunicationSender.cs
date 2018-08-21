using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.Application.Infrastructure.Messaging
{
    public class EndpointCommunicationSender : EndpointCommunicationBase, IEndpointCommunicationSender
    {
        public EndpointCommunicationSender(string endpointName, string storageConnectionString, ILifetimeScope lifetimeScope) 
            : base(endpointName, storageConnectionString, lifetimeScope)
        {
        }

        protected override void OnConfigure(EndpointConfiguration configuration, TransportExtensions<AzureStorageQueueTransport> transport)
        {
            //transport.Routing().RouteToEndpoint(typeof(T).Assembly, _destinationEndpointName);
            configuration.SendOnly();
        }

        public async Task Send<T>(T message) where T : IPaymentsMessage
        {
            if (EndpointInstance == null)
                await StartEndpoint().ConfigureAwait(false);

            await EndpointInstance.Send(message).ConfigureAwait(false);
        }
    }
}