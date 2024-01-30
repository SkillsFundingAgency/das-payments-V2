using System;
using System.Collections.Generic;
using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Monitoring.Metrics.SubmissionService
{
    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class SubmissionService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger logger;
        private IStatelessEndpointCommunicationListener listener;

        public SubmissionService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger logger)
            : base(context)
        {
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            logger.LogInfo("Creating Service Instance Listeners For SubmissionService");


            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>
                    listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
            };
        }
    }
}