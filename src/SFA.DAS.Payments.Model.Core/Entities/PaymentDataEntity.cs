using System;

namespace SFA.DAS.Payments.Model.Core.Entities
{
    public class PaymentDataEntity
    {
        public Guid Id { get; set; }
        public long Ukprn { get; set; }

        public int ContractType { get; set; }
        public int TransactionType { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public int FundingSource { get; set; }
        public decimal Amount { get; set; }

        public int DeliveryPeriodMonth { get; set; }
        public int DeliveryPeriodYear { get; set; }

        public string CollectionPeriodName { get; set; }
        public int CollectionPeriodMonth { get; set; }
        public int CollectionPeriodYear { get; set; }

        public string LearnerReferenceNumber { get; set; }
        public string LearnAimReference { get; set; }

        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        public int StandardCode { get; set; }
        public int ProgrammeType { get; set; }
    }
}
