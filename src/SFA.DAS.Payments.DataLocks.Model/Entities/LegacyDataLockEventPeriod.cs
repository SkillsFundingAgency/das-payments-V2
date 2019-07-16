using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class LegacyDataLockEventPeriod
    {
        public Guid DataLockEventId { get; set; }
        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public string CommitmentVersion { get; set; }
        public bool IsPayable { get; set; }
        public int TransactionType { get; set; }
        public int TransactionTypesFlag { get; set; }
    }
}