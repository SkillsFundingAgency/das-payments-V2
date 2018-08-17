using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;
using System.Collections.Generic;
using System.Fabric;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService
{
    public class RequiredPaymentsProxyService : StatelessService
    {
        private IEndpointCommunicationListener<IPayableEarningEvent> _listener;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IPaymentLogger _paymentLogger;

        public RequiredPaymentsProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            _lifetimeScope = lifetimeScope;
            _paymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            _paymentLogger.LogInfo($"Creating Service Instance Listeners For RequiredPaymentsProxyService");

            return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context =>_listener = _lifetimeScope.Resolve<IEndpointCommunicationListener<IPayableEarningEvent>>())
                };
        }
    }
}