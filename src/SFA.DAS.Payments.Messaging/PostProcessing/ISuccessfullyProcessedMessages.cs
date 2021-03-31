using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.Messaging.PostProcessing
{
    public interface ISuccessfullyProcessedMessages
    {
        Task Process(Type groupType, List<object> messages, CancellationToken cancellationToken);
    }
}