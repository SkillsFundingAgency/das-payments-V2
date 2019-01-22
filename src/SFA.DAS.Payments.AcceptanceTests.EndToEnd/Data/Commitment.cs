using System;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Commitment
    {
        public string Identifier { get; set; } = "Commitment 1";
        public long CommitmentId { get; set; }
        public long SequenceId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal AgreedCost { get; set; }
        public long StandardCode { get; set; }
        public long ProgrammeType { get; set; }
        public long FrameworkCode { get; set; }
        public long PathwayCode { get; set; }
        public CommitmentPaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusDescription { get; set; }
        public int Priority { get; set; }
        public DateTime EffectiveFromDate  { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string LegalEntityName { get; set; }
        public long TransferSendingEmployerAccountId { get; set; }
        public DateTime? TransferApprovalDate { get; set; }
        public DateTime? PausedOnDate { get; set; }
        public DateTime? WithdrawnOnDate { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
    }

    public enum CommitmentPaymentStatus
    {

    }
}
