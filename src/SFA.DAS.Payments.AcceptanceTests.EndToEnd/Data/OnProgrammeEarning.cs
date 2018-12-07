using System.Collections.Concurrent;
using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class Earning
    {
        public Earning()
        {
            Values = new Dictionary<TransactionType, decimal>();
        }

        private CalendarPeriod deliveryPeriod;

        public CalendarPeriod DeliveryCalendarPeriod =>
            deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

        public string DeliveryPeriod { get; set; }
        public decimal OnProgramme { get; set; }
        public decimal Completion { get; set; }
        public decimal Balancing { get; set; }
        public string FundingLineType { get; set; } 
        public string SfaContributionPercentage { get; set; } 

        public string LearnerId { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public IDictionary<TransactionType, decimal> Values { get; set; }
    }


    //public class OnProgrammeEarning
    //{
    //    private CalendarPeriod deliveryPeriod;

    //    public CalendarPeriod DeliveryCalendarPeriod =>
    //        deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

    //    public string DeliveryPeriod { get; set; }
    //    public decimal OnProgramme { get; set; }
    //    public decimal Completion { get; set; }
    //    public decimal Balancing { get; set; }
    //    public string FundingLineType { get; set; }
    //    public string SfaContributionPercentage { get; set; }
    //    public string LearnerId { get; set; }
    //}
}