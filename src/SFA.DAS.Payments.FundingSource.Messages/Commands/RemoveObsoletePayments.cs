using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.FundingSource.Messages.Commands
{
    public class RemoveObsoletePayments
    {
        public Guid LastEarningEventId { get; set; }
        public long AccountId { get; set; }
        public Guid LastDataLockEventId { get; set; }
    }
}
