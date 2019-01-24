using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core
{

    public class StatelessEndpointCommunicationListener: IStatelessEndpointCommunicationListener
    {
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IApplicationConfiguration config;
        private IEndpointInstance endpointInstance;

        public StatelessEndpointCommunicationListener(IEndpointInstanceFactory endpointInstanceFactory, IApplicationConfiguration config)
        {
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            return config.EndpointName;
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