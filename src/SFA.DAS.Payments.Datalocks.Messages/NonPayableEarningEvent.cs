using System.Collections.ObjectModel;

namespace SFA.DAS.Payments.DataLocks.Messages
{
    public class NonPayableEarningEvent : DataLockableEarningEvent
    {
        public ReadOnlyCollection<DataLockErrorCode> Errors { get; set; }
    }
}