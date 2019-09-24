using System;

namespace SFA.DAS.Payments.DataLocks.Model.Entities
{
    public class LegacyDataLockEventCommitmentVersion
    {
        public Guid DataLockEventId { get; set; }
        public string CommitmentVersion { get; set; }
        public DateTime CommitmentStartDate { get; set; }
        public long? CommitmentStandardCode { get; set; }
        public int? CommitmentProgrammeType { get; set; }
        public int? CommitmentFrameworkCode { get; set; }
        public int? CommitmentPathwayCode { get; set; }
        public decimal CommitmentNegotiatedPrice { get; set; }
        public DateTime CommitmentEffectiveDate { get; set; }
    }
}
