using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class DataLockEventNonPayablePeriodFailureModel
    {
        public long Id { get; set; }
        public Guid DataLockEventNonPayablePeriodId { get; set; }
        public virtual DataLockEventNonPayablePeriodModel DataLockEventNonPayablePeriod { get; set; }
        public DataLockErrorCode DataLockFailure { get; set; }
        public virtual ApprenticeshipModel Apprenticeship { get; set; }
        public long? ApprenticeshipId { get; set; }
    }
}