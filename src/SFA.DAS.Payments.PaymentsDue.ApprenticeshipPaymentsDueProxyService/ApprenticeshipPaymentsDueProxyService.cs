using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService
{
    public class ApprenticeshipPaymentsDueProxyService : StatelessService
    {
        private IEndpointCommunicationListener<IPayableEarningEvent> _listener;
        private readonly ILifetimeScope _lifetimeScope;

        public ApprenticeshipPaymentsDueProxyService(StatelessServiceContext context, ILifetimeScope lifetimeScope)
            : base(context)
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

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                 //await _listener.OpenAsync(cancellationToken);//.RunAsync();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error running actor proxy service. Error: {ex.Message}. Ex: {ex}");
                throw;
            }
        }
    }
}
