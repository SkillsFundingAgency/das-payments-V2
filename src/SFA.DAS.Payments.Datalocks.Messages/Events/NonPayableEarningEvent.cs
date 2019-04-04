using System.Collections.ObjectModel;
using SFA.DAS.Payments.DataLocks.Domain.Models;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class NonPayableEarningEvent : DataLockEvent
    {
        public ReadOnlyCollection<DataLockErrorCode> Errors { get; set; }
    }
}