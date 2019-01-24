using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.ServiceFabric.Core;
using System.Collections.Generic;
using System.Fabric;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService
{
    public class RequiredPaymentsProxyService : StatelessService
    {
        private IStatelessEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;

        public RequiredPaymentsProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            paymentLogger.LogInfo("Creating Service Instance Listeners For RequiredPaymentsProxyService");

            return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context =>listener = lifetimeScope.Resolve<IStatelessEndpointCommunicationListener>())
                };
        }
    }
}