using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Commitment
    {
        public string Identifier { get; set; } = "Commitment 1";
        
        public string LearnerId { get; set; }
        public string Employer { get; set; }
        public string Provider { get; set; }

        public long CommitmentId { get; set; }
        public long SequenceId { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long AccountId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal AgreedPrice { get; set; }
        public long StandardCode { get; set; }
        public long ProgrammeType { get; set; }
        public long FrameworkCode { get; set; }
        public long PathwayCode { get; set; }
        public CommitmentPaymentStatus PaymentStatus { get; set; } = CommitmentPaymentStatus.Active;
        public string PaymentStatusDescription { get; set; } = "Test Status Description";
        public int Priority { get; set; }
        public DateTime EffectiveFromDate  { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string LegalEntityName { get; set; } = "Test Legal Entity";
        public long TransferSendingEmployerAccountId { get; set; }
        public DateTime? TransferApprovalDate { get; set; }
        public DateTime? PausedOnDate { get; set; }
        public DateTime? WithdrawnOnDate { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long VersionId { get; set; } = 1L;
    }
}
