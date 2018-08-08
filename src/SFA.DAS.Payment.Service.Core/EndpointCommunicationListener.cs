using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using NServiceBus;
using NServiceBus.Features;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public class EndpointCommunicationListener<T> : EndpointCommunicationBase<T>, ICommunicationListener 
        where T : IPaymentsMessage
    {
        public EndpointCommunicationListener(string endpointName, string storageConnectionString) 
            : base(endpointName, storageConnectionString)
        {
        }

        public async Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            await StartEndpoint();
            return EndpointName;
        }

        //public async Task RunAsync()
        //{
        //    try
        //    {
        //        // TODO: do stuff

        //        //EndpointInstance = await Endpoint.Start(_endpointConfiguration)
        //        //    .ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceError($"Error starting endpoint. Error: {ex.Message}. Ex: {ex}");
        //        throw;
        //    }
        //}

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            return EndpointInstance.Stop();
        }

        public void Abort()
        {
            CloseAsync(CancellationToken.None);
        }
    }
}