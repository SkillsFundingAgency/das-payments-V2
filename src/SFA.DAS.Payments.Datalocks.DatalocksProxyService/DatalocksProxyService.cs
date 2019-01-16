using System.Collections.Generic;
using System.Fabric;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.Datalocks.DatalocksProxyService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class DatalocksProxyService : StatelessService
    {
        private IEndpointCommunicationListener listener;
        private readonly ILifetimeScope lifetimeScope;
        private readonly IPaymentLogger paymentLogger;

        public DatalocksProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope, IPaymentLogger paymentLogger) : base(context)
        {
            this.lifetimeScope = lifetimeScope;
            this.paymentLogger = paymentLogger;
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            paymentLogger.LogInfo("Creating Service Instance Listeners For DatalocksProxyService");

            return new List<ServiceInstanceListener>
            {
                new ServiceInstanceListener(context =>listener = lifetimeScope.Resolve<IEndpointCommunicationListener>())
            };
        }
    }
}
