using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Messaging.PostProcessing
{
    public interface IFailedProcessingMessage
    {
        Task Process(object message, Exception e, CancellationToken cancellationToken);
    }
}