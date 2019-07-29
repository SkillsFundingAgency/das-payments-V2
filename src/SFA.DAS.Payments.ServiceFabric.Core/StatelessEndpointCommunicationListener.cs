using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;

namespace SFA.DAS.Payments.ServiceFabric.Core
{

    public class StatelessEndpointCommunicationListener : IStatelessEndpointCommunicationListener
    {
        private readonly IEndpointInstanceFactory endpointInstanceFactory;
        private readonly IApplicationConfiguration config;
        private readonly IPaymentLogger logger;
        private IEndpointInstance endpointInstance;

        public StatelessEndpointCommunicationListener(IEndpointInstanceFactory endpointInstanceFactory, IApplicationConfiguration config, IPaymentLogger logger)
        {
            this.endpointInstanceFactory = endpointInstanceFactory ?? throw new ArgumentNullException(nameof(endpointInstanceFactory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Opens the asynchronous.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            try
            {
                logger.LogDebug($"Opening endpoint: {config.EndpointName}");
                endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
                logger.LogInfo($"Finished opening endpoint listener: {config.EndpointName}");
                return config.EndpointName;
            }
            catch (Exception e)
            {
                logger.LogFatal($"Cannot start the endpoint: '{config.EndpointName}'.  Error: {e.Message}", e);
                throw;
            }
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