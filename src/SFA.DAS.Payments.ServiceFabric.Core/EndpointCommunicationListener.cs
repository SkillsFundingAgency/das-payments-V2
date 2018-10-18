using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class EndpointCommunicationListener: IEndpointCommunicationListener
    {
        private readonly EndpointConfiguration endpointConfiguration;
        private readonly IApplicationConfiguration config;
        private IEndpointInstance endpointInstance;

        public EndpointCommunicationListener(EndpointConfiguration endpointConfiguration, IApplicationConfiguration config)
        {
            this.endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
            return config.EndpointName;
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return endpointInstance.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }
    }
}