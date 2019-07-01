using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Interfaces
{
    public interface IDataLockEventProcessor
    {
        Task<DataLockStatusChanged> ProcessDataLockEvent(DataLockEvent dataLockEvent);
    }
}
