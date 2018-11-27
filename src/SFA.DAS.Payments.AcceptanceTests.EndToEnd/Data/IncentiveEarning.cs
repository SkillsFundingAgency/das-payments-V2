using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class IncentiveEarning
    {
        private CalendarPeriod deliveryPeriod;

        public CalendarPeriod DeliveryCalendarPeriod =>
            deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

        public string DeliveryPeriod { get; set; }
        public decimal First16To18EmployerIncentive { get; set; }
        public decimal First16To18ProviderIncentive {get;set;}
        public decimal Second16To18EmployerIncentive { get; set; }
        public decimal Second16To18ProviderIncentive { get; set; }
        public decimal OnProgramme16To18FrameworkUplift { get; set; }
        public decimal Completion16To18FrameworkUplift { get; set; }
        public decimal Balancing16To18FrameworkUplift { get; set; }
        public decimal FirstDisadvantagePayment { get; set; }
        public decimal SecondDisadvantagePayment { get; set; }
        public decimal LearningSupport { get; set; }
        public string LearnerId { get; set; }
    }
}