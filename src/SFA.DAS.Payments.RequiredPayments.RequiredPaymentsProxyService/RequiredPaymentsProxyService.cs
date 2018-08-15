using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.ServiceFabric.Core;

namespace SFA.DAS.Payments.RequiredPayments.RequiredPaymentsProxyService
{

    public class RequiredPaymentsProxyService : StatelessService
    {
        private IEndpointCommunicationListener<IPayableEarningEvent> _listener;
        private readonly ILifetimeScope _lifetimeScope;

        public RequiredPaymentsProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope) : base(context)
        {
            _lifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new List<ServiceInstanceListener>
            {
                // TODO: put this stuff to config
                new ServiceInstanceListener(context =>
                    _listener = _lifetimeScope.Resolve<IEndpointCommunicationListener<IPayableEarningEvent>>()
                )
            };
        }

      

    }


}
