using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payment.ServiceFabric.Core
{
    public interface IEndpointCommunicationSender<T> where T : IPaymentsMessage
    {
        Task Send(T message);
    }
}