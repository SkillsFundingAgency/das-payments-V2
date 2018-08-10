using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using SFA.DAS.Payment.ServiceFabric.Core;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService
{
    public class ApprenticeshipPaymentsDueProxyService : StatelessService
    {
        private EndpointCommunicationListener<IPayableEarningEvent> _listener;
        private IServiceConfig _config;

        public ApprenticeshipPaymentsDueProxyService(StatelessServiceContext context, IServiceConfig config)
            : base(context)
        {
            _config = config;
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
                    _listener = new EndpointCommunicationListener<IPayableEarningEvent>(_config.IncomingEndpointName, _config.StorageConnectionString)
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
