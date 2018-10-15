using System.Collections.Generic;
using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.PaymentsDue.PaymentsDueService
{
    public class PaymentsDueService : StatelessService
    {
        private IEndpointCommunicationListener listener;
        private readonly IPaymentLogger paymentLogger;
        private readonly ILifetimeScope lifetimeScope;

        public PaymentsDueService(StatelessServiceContext context, IPaymentLogger paymentLogger, ILifetimeScope lifetimeScope)
            : base(context)
        {
            this.paymentLogger = paymentLogger;
            this.lifetimeScope = lifetimeScope;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            paymentLogger.LogInfo("Creating Service Instance Listeners For PaymentsDueService");

            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context => listener = lifetimeScope.Resolve<IEndpointCommunicationListener>())
            };
        }
    }
}