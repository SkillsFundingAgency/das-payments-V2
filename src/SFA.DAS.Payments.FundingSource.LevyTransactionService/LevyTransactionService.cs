using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.FundingSource.LevyTransactionService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class LevyTransactionService : StatelessService
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;

        public LevyTransactionService(StatelessServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            try
            {
                return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context => lifetimeScope.Resolve<IStatelessServiceBusBatchCommunicationListener>())
                };
            }
            catch (Exception e)
            {
                logger.LogError($"Error starting the service instance listener: {e.Message}", e);
                throw;
            }
        }
    }
}
