using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core.Audit
{
    public class EarningEventPeriodModel
    {
        public long Id { get; set; }
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public TransactionType TransactionType { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
        public decimal? SfaContributionPercentage { get; set; }
        public DateTime? CensusDate { get; set; }
    }
}