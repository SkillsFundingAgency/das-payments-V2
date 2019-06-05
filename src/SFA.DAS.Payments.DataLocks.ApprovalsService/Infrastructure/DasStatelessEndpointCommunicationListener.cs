using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure
{
    public interface IDasStatelessEndpointCommunicationListener : IStatelessEndpointCommunicationListener { }

    public class DasStatelessEndpointCommunicationListener : IDasStatelessEndpointCommunicationListener
    {
        private readonly EndpointConfiguration endpointConfiguration;
        private readonly IApplicationConfiguration config;
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IEndpointInstance endpointInstance;

        public DasStatelessEndpointCommunicationListener(EndpointConfiguration endpointConfiguration, IApplicationConfiguration config, IPaymentLogger logger, ILifetimeScope lifetimeScope)
        {
            this.endpointConfiguration = endpointConfiguration ?? throw new ArgumentNullException(nameof(endpointConfiguration));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
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
                if (endpointInstance == null)
                {
                    endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(lifetimeScope));
                    endpointInstance = await Endpoint.Start(endpointConfiguration).ConfigureAwait(false);
                }
                logger.LogInfo($"Finished opening endpoint listener: {config.EndpointName}");
                return "das endpoint comms listener";
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