using System;
using System.Collections.Generic;
using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.FundingSource.LevyAccountBalanceService
{
    public class LevyAccountBalanceService : StatelessService
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private IStatelessEndpointCommunicationListener listener;

        protected LevyAccountBalanceService(StatelessServiceContext context,
            IPaymentLogger logger,
            ILifetimeScope lifetimeScope,
            IConfigurationHelper configurationHelper) : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            logger.LogInfo("Creating Service Instance Listeners For Levy Account Balance Service");
            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context => listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }
    }
}
