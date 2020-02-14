using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobService
{
    public interface IInProgressMessageRepository
    {
        Task<List<InProgressMessage>> GetInProgressMessages(long jobId, CancellationToken cancellationToken);
        Task RemoveInProgressMessages(long jobId, List<Guid> messageIdentifiers, CancellationToken cancellationToken);
        Task StoreInProgressMessages(long jobId, List<InProgressMessage> inProgressMessages, CancellationToken cancellationToken);
    }
}