using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public interface ICompletedMessageRepository
    {
        Task<List<CompletedMessage>> GetCompletedMessages(long jobId, CancellationToken cancellationToken);
        Task RemoveCompletedMessages(long jobId, List<Guid> completedMessages, CancellationToken cancellationToken);
        Task StoreCompletedMessage(CompletedMessage completedMessage, CancellationToken cancellationToken);
    }
}