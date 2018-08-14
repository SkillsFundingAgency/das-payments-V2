using System.Threading;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public class EndpointCommunicationListener<T> : EndpointCommunicationBase<T>, IEndpointCommunicationListener<T>
        where T : IPaymentsMessage
    {
        public EndpointCommunicationListener(string endpointName, string storageConnectionString, ILifetimeScope lifetimeScope) 
            : base(endpointName, storageConnectionString, lifetimeScope)
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