using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Earning
    {
        public byte DeliveryCalendarPeriod => new DeliveryPeriodBuilder().WithSpecDate(DeliveryPeriod).Build();

        public string DeliveryPeriod { get; set; }
        public decimal OnProgramme { get; set; }
        public decimal Completion { get; set; }
        public decimal Balancing { get; set; }
        public long? AimSequenceNumber { get; set; }
        public string SfaContributionPercentage { get; set; }

        public string LearnerId { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public IDictionary<TransactionType, decimal> Values { get; set; } = new Dictionary<TransactionType, decimal>();
        public long Uln { get; set; }

        public long Ukprn { get; set; }
    }
}