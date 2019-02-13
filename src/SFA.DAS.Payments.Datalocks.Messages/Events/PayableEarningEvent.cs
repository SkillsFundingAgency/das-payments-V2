namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class PayableEarningEvent : DataLockEvent
    {
        public int Priority { get; set; }
        public long EmployerAccountId { get; set; }
        public long CommitmentId { get; set; }
        public int CommitmentVersion { get; set; }
    }
}
