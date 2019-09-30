using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Application.Messaging
{
    public interface IHandleMessageBatches<T>
    {
        Task Handle(IList<T> messages, CancellationToken cancellationToken);
    }
}