using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.ProviderAdjustments.Service
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Service : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger logger;
        
        public Service(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var initialised = false;
            
            try
            {
                logger.LogDebug("Starting the Provider Ajustments service.");
                initialised = true;
                logger.LogInfo("Started the Provider Adjustments service.");
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (Exception exception) when (!(exception is TaskCanceledException))
            {
                // Ignore, as an exception is only really thrown on cancellation of the token.
                logger.LogError($"Reference Data Stateless Service Exception. Error: {exception}.", exception);
            }
            finally
            {
                if (initialised)
                {
                    logger.LogInfo("Provider Adjustments Stateless Service End");
                }
            }
        }
    }
}
