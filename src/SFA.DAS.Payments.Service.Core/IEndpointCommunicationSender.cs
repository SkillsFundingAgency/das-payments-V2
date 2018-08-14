using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IEndpointCommunicationSender<T> where T : IPaymentsMessage
    {
        Task Send(T message);
    }
}