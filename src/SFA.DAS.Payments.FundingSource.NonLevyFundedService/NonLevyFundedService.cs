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
        private readonly ILifetimeScope LifetimeScope;
        private readonly IPaymentLogger PaymentLogger;

        public NonLevyFundedService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            LifetimeScope = lifetimeScope;
            PaymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            PaymentLogger.LogInfo("Creating Service Instance Listeners For NonLevyFundedService");

            return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context =>LifetimeScope.Resolve<IEndpointCommunicationListener>())
                };
        }
    }
}