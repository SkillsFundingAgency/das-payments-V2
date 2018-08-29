using System.Threading;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Application.Infrastructure.Messaging;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class EndpointCommunicationListener: EndpointCommunicationBase, IEndpointCommunicationListener
    {
        public EndpointCommunicationListener(string endpointName, string storageConnectionString, ILifetimeScope lifetimeScope) 
            : base(endpointName, storageConnectionString, lifetimeScope)
        {
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            await StartEndpoint();
            return EndpointName;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return EndpointInstance.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }
    }
}