using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.ServiceFabric.Core
{
    public interface IEndpointCommunicationSender
    {
        Task Send<T>(T message) where T : IPaymentsMessage;
    }
}