using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class CommitmentModel
    {
        public long CommitmentId { get; set; }
        public string SequenceId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long AccountId { get; set; }
        public long AccountSequenceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AgreedCost { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public CommitmentPaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusDescription { get; set; }
        public int Priority { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string LegalEntityName { get; set; }
        public long? TransferSendingEmployerAccountId { get; set; }
        public DateTime? TransferApprovalDate { get; set; }
        public DateTime? PausedOnDate { get; set; }
        public DateTime? WithdrawnOnDate { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long VersionId { get; set; }
    }

    public enum CommitmentPaymentStatus
    {
        Active = 1,
        NotSure = 2,
        Withdrawn = 3,
    }
}
