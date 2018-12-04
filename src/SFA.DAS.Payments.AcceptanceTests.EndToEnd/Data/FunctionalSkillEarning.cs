using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class FunctionalSkillEarning
    {
        private CalendarPeriod deliveryPeriod;

        public CalendarPeriod DeliveryCalendarPeriod =>
            deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

        public string DeliveryPeriod { get; set; }

        public decimal? OnProgrammeMathsAndEnglish { get; set; }

        public decimal? BalancingMathsAndEnglish { get; set; }

        public string LearnerId { get; set; }

        public bool Specified => OnProgrammeMathsAndEnglish.HasValue || BalancingMathsAndEnglish.HasValue;
    }
}