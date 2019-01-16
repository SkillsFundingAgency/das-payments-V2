using System;

namespace SFA.DAS.Payments.Audit.AcceptanceTests.Data.Entities
{
    public class FundingSourceEvent
    {
        public long Id { get; set; } // Id (Primary key)
        public System.Guid EventId { get; set; } // EventId
        public System.Guid RequiredPaymentEventId { get; set; } // RequiredPaymentEventId
        public string PriceEpisodeIdentifier { get; set; } // PriceEpisodeIdentifier (length: 50)
        public byte ContractType { get; set; } // ContractType
        public byte TransactionType { get; set; } // TransactionType
        public byte FundingSourceType { get; set; } // FundingSourceType
        public decimal Amount { get; set; } // Amount
        public byte CollectionPeriod { get; set; } // CollectionPeriod
        public string CollectionYear { get; set; } // CollectionYear (length: 4)
        public byte DeliveryPeriod { get; set; } // DeliveryPeriod
        public string LearnerReferenceNumber { get; set; } // LearnerReferenceNumber (length: 50)
        public long LearnerUln { get; set; } // LearnerUln
        public string LearningAimReference { get; set; } // LearningAimReference (length: 8)
        public int LearningAimProgrammeType { get; set; } // LearningAimProgrammeType
        public int LearningAimStandardCode { get; set; } // LearningAimStandardCode
        public int LearningAimFrameworkCode { get; set; } // LearningAimFrameworkCode
        public int LearningAimPathwayCode { get; set; } // LearningAimPathwayCode
        public string LearningAimFundingLineType { get; set; } // LearningAimFundingLineType (length: 100)
        public string AgreementId { get; set; } // AgreementId (length: 255)
        public long Ukprn { get; set; } // Ukprn
        public DateTime IlrSubmissionDateTime { get; set; } // IlrSubmissionDateTime
        public long JobId { get; set; } // JobId
        public DateTimeOffset EventTime { get; set; } // EventTime
    }
}