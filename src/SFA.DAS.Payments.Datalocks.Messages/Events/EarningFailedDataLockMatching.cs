using SFA.DAS.Payments.Messages.Common;
using System;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class EarningFailedDataLockMatching : DataLockEvent, IMonitoredMessage
    {
        public DateTime StartDate { get; set; }
    }
}