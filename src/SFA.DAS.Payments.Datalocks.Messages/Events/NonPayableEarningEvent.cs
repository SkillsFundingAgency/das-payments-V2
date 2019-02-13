using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class NonPayableEarningEvent : DataLockEvent
    {
        public ReadOnlyCollection<DataLockErrorCode> Errors { get; set; }
    }
}