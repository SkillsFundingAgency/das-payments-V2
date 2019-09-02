using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Ioc;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.DataLocks.Application.Infrastructure;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.DataLocks.ApprovalsService.Infrastructure
{
    public interface IDasStatelessEndpointCommunicationListener : IStatelessEndpointCommunicationListener { }

    public class DasStatelessEndpointCommunicationListener : IDasStatelessEndpointCommunicationListener
    {
        private readonly IApplicationConfiguration config;
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IEndpointInstance endpointInstance;
        private IDasMessageSessionFactory dasMessageSessionFactory;

        public DasStatelessEndpointCommunicationListener(IApplicationConfiguration config, IPaymentLogger logger, ILifetimeScope lifetimeScope, IDasMessageSessionFactory dasMessageSessionFactory)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.dasMessageSessionFactory = dasMessageSessionFactory ?? throw new ArgumentNullException(nameof(dasMessageSessionFactory));
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
                    var endpointConfiguration = CreateEndpointConfiguration();
                    //endpointConfiguration.UseContainer<AutofacBuilder>(c => c.ExistingLifetimeScope(lifetimeScope));
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

        private EndpointConfiguration CreateEndpointConfiguration()
        {
            var endpointConfiguration = dasMessageSessionFactory.InitialiseEndpointConfig();

            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(lifetimeScope.Resolve<IContainerScopeFactory>()));
            endpointConfiguration.RegisterComponents(cfg => cfg.RegisterSingleton(lifetimeScope.Resolve<IEndpointInstanceFactory>()));
            
            return endpointConfiguration;
        }
    }
}