using System;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class EarningFailedDataLockMatching : DataLockEvent
    {
        public DateTime StartDate { get; set; }
    }
}