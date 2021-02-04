using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class LevyTransactionModel
    {
        public long Id { get; set; }
        public long Ukprn { get; set; }
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
        public byte DeliveryPeriod { get; set; }
        public long JobId { get; set; }
        public long AccountId { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid RequiredPaymentEventId { get; set; }
        public Guid EarningEventId { get; set; }
        public decimal Amount { get; set; }
        public string MessagePayload { get; set; }
        public string MessageType { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public long FundingAccountId { get; set; }
        public TransactionType TransactionType { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public long? LearnerUln { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public string LearningAimReference { get; set; }
        public int? LearningAimProgrammeType { get; set; }
        public int? LearningAimStandardCode { get; set; }
        public int? LearningAimFrameworkCode { get; set; }
        public int? LearningAimPathwayCode { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public DateTime? LearningStartDate { get; set; }
        public long? ApprenticeshipId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
    }
}