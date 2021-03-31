using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Messaging.PostProcessing
{
    public interface IInterceptFailedMessages
    {
        Task Process(object message, Exception messageException, CancellationToken cancellationToken);
    }
}