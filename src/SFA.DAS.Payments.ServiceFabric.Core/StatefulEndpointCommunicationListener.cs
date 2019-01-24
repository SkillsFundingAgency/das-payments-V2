using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class StatefulEndpointCommunicationListener: IStatefulEndpointCommunicationListener
    {
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IApplicationConfiguration config;
        private IEndpointInstance endpointInstance;

        public StatefulEndpointCommunicationListener(IEndpointInstanceFactory endpointInstanceFactory, IApplicationConfiguration config)
        {
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(config.EndpointName);
        }

        public async Task RunAsync()
        {
            endpointInstance = await endpointInstanceFactory
                .GetEndpointInstance()
                .ConfigureAwait(false);
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return endpointInstance?.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }
    }
}