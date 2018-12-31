using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Earning
    {
        private CalendarPeriod deliveryPeriod;

        public CalendarPeriod DeliveryCalendarPeriod =>
            deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

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
    }
}