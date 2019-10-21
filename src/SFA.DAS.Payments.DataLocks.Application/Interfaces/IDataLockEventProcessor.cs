using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Interfaces
{
    public interface IDataLockEventProcessor
    {
        Task<List<DataLockStatusChanged>> ProcessPayableEarning(DataLockEvent dataLockEvent);
        Task<List<DataLockStatusChanged>> ProcessDataLockFailure(DataLockEvent dataLockEvent);
    }
}
