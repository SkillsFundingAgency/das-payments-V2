using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;
using System.Collections.Generic;
using System.Fabric;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;

namespace SFA.DAS.Payments.FundingSource.NonLevyFundedService
{
    public class NonLevyFundedService : StatelessService
    {
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;
        private readonly IContractType2RequiredPaymentEventFundingSourceService contractType2RequiredPaymentService;

        public NonLevyFundedService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger,
            IContractType2RequiredPaymentEventFundingSourceService contractType2RequiredPaymentService) : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
            this.contractType2RequiredPaymentService = contractType2RequiredPaymentService;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            paymentLogger.LogInfo("Creating Service Instance Listeners For NonLevyFundedService");

            return new List<ServiceInstanceListener>
                {
                    new ServiceInstanceListener(context =>lifetimeScope.Resolve<IEndpointCommunicationListener>())
                };
        }
    }
}