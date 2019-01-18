using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.Messages
{
    public class PayableEarningEvent : DataLockableEarningEvent
    {
        public int Priority { get; set; }
        public long EmployerAccountId { get; set; }
        public int CommitmentVersion { get; set; }
    }
}
