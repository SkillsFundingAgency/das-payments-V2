using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.Application.Infrastructure.Messaging
{
    public interface IEndpointCommunicationSender
    {
        Task Send<T>(T message) where T : IPaymentsMessage;
    }
}