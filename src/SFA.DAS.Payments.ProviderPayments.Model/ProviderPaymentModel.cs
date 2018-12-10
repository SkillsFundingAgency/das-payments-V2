using System;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Model
{
    public class ProviderPaymentModel: IPaymentsEventModel
    {
        public long Id { get; set; }
        public Guid FundingSourceId { get; set; }
        public Guid ExternalId { get; set; }
        public long Ukprn { get; set; }
        public Guid EventId { get; set; }
        public byte CollectionPeriod { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public long LearnerUln { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal Amount { get; set; }
        public string LearningAimReference { get; set; }
        public int LearningAimProgrammeType { get; set; }
        public int LearningAimStandardCode { get; set; }
        public int LearningAimFrameworkCode { get; set; }
        public int LearningAimPathwayCode { get; set; }
        public string LearningAimFundingLineType { get; set; }
        public ContractType ContractType { get; set; }
        public TransactionType TransactionType { get; set; }
        public FundingSourceType FundingSource { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public string CollectionYear { get; set; }
    }
}