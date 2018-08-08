using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public class EndpointCommunicationSender<T> : EndpointCommunicationBase<T>, IEndpointCommunicationSender<T> where T : IPaymentsMessage
    {
        public EndpointCommunicationSender(string endpointName, string storageConnectionString) 
            : base(endpointName, storageConnectionString)
        {
        }

        protected override void OnConfigure(EndpointConfiguration configuration)
        {
            configuration.SendOnly();
        }

        public async Task Send(T message)
        {
            await EndpointInstance.Send(message).ConfigureAwait(false);
        }
    }
}