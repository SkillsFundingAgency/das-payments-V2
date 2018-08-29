using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;
using System.Collections.Generic;
using System.Fabric;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService
{
    public class NonLevyFundedService : StatelessService
    {
        private IEndpointCommunicationListener _listener;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IPaymentLogger _paymentLogger;

        public NonLevyFundedService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            _lifetimeScope = lifetimeScope;
            _paymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            _paymentLogger.LogInfo("Creating Service Instance Listeners For NonLevyFundedService");

            return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context =>_listener = _lifetimeScope.Resolve<IEndpointCommunicationListener>())
                };
        }
    }
}