using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class DataLockEventModel : PaymentsEventModel
    {
        public long Id { get; set; }
        public ContractType ContractType { get; set; }
        public string AgreementId { get; set; }
        public long? LearningAimSequenceNumber { get; set; }
        public virtual List<DataLockEventNonPayablePeriodModel> NonPayablePeriods { get; set; }
        public string IlrFileName { get; set; }
        public decimal? SfaContributionPercentage { get; set; }
        public string EventType { get; set; }
        public bool IsPayable { get; set; }
        public DataLockSource DataLockSource { get; set; }
    }
}