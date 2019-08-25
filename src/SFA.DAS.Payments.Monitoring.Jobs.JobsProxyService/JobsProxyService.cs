using System;
using System.Collections.Generic;
using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService
{
    public class JobsProxyService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger logger;

        public JobsProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            logger.LogInfo("Creating Service Instance Listeners For Job Proxy Service");

            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context => lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }
    }
}
