using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Interfaces
{
    public interface IDataLockProcessor
    {
        Task<List<DataLockEvent>> GetPaymentEvents(ApprenticeshipContractType1EarningEvent earningEvent,CancellationToken cancellationToken);
    }
}