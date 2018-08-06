using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.PaymentsDue.ApprenticeshipPaymentsDueProxyService
{
    public class EndpointCommunicationListener : ICommunicationListener
    {
        private IEndpointInstance _endpointInstance;
        private EndpointConfiguration _endpointConfiguration;

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            var endpointName = "sfa-das-payments-paymentsdue-proxyservice";

            _endpointConfiguration = new EndpointConfiguration(endpointName);

            var conventions = _endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(type => typeof(PaymentsCommand).IsAssignableFrom(type));
            conventions.DefiningEventsAs(type => typeof(IPaymentsEvent).IsAssignableFrom(type));
            conventions.DefiningMessagesAs(type => typeof(IPaymentsMessage).IsAssignableFrom(type));

            var persistence = _endpointConfiguration.UsePersistence<AzureStoragePersistence>();
            persistence.ConnectionString("UseDevelopmentStorage=true");

            _endpointConfiguration.DisableFeature<TimeoutManager>();
            _endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
            
            var transport = _endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
            transport.ConnectionString("UseDevelopmentStorage=true");
            transport.BatchSize(1);
            transport.DegreeOfReceiveParallelism(1);

            _endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
            _endpointConfiguration.EnableInstallers();

            return Task.FromResult(endpointName);
        }

        public async Task RunAsync()
        {
            try
            {
                _endpointInstance = await Endpoint.Start(_endpointConfiguration)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Error starting endpoint. Error: {ex.Message}. Ex: {ex}");
                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return _endpointInstance.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }
    }
}
