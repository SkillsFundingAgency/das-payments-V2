using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class PaymentDataEntity
    {
        public Guid Id { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal Amount { get; set; }
        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }
        public int DeliveryPeriodMonth { get; set; }
        public int DeliveryPeriodYear { get; set; }
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public decimal LearningAimAgreedPrice { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public long JobId { get; set; }
        public int ContractType { get; set; }
        public int TransactionType { get; set; }
        public int FundingSource { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public decimal SfaContributionPercentage { get; set; }
    }
}
