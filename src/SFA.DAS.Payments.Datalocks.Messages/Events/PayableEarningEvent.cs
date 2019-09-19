using System;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class PayableEarningEvent : DataLockEvent, ILeafLevelMessage
    {
        public DateTime StartDate { get; set; }
    }
}
