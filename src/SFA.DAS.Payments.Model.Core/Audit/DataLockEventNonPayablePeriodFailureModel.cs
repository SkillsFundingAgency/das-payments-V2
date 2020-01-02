namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class DataLockEventNonPayablePeriodFailureModel
    {
        public long Id { get; set; }
        public virtual DataLockEventNonPayablePeriodModel DataLockEventNonPayablePeriod { get; set; }
        public DataLockErrorCode DataLockFailure { get; set; }
    }
}