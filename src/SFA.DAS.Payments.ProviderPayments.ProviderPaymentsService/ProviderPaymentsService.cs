using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.ServiceFabric.Core;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService
{

    public class ProviderPaymentsService : StatelessService
    {
        private readonly IPaymentLogger logger;
        private readonly ILifetimeScope lifetimeScope;
        private ICommunicationListener listener;


        public ProviderPaymentsService(StatelessServiceContext context, IPaymentLogger logger, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            logger.LogInfo("Creating Service Instance Listener For Audit.FundingSourceService");
            var listeners = new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context => listener = lifetimeScope.Resolve<IStatelessServiceBusBatchCommunicationListener>())
            };
            logger.LogInfo("Created Service Instance Listener For Audit.FundingSourceService");
            return listeners;
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(RunSendOnlyEndpoint());
        }

        private async Task RunSendOnlyEndpoint()
        {
            var endpoint = lifetimeScope.Resolve<EndpointConfiguration>();
            endpoint.SendOnly();
            var factory = lifetimeScope.Resolve<IEndpointInstanceFactory>();
            await factory.GetEndpointInstance();
        }
    }
}
