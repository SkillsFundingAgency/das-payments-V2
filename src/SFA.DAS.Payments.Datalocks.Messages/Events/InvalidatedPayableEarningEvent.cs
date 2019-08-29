using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class InvalidatedPayableEarningEvent
    {
        public Guid LastEarningEventId { get; set; }
        public Guid LastDataLockEventId { get; set; }
        public List<long> AccountIds { get; set; }
    }
}
