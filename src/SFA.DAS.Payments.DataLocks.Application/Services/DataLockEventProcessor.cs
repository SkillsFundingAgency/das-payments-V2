using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class DataLockEventProcessor : IDataLockEventProcessor
    {
        public Task<DataLockStatusChanged> ProcessDataLockEvent(DataLockEvent dataLockEvent)
        {
            return null;
        }
    }
}
