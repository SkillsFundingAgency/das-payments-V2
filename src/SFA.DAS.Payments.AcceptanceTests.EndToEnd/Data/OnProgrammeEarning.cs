using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class OnProgrammeEarning
    {
        private CalendarPeriod deliveryPeriod;

        public CalendarPeriod DeliveryCalendarPeriod =>
            deliveryPeriod ?? (deliveryPeriod = DeliveryPeriod.ToDate().ToCalendarPeriod());

        public string DeliveryPeriod { get;set; }
        public decimal OnProgramme { get; set; }
        public decimal Completion { get; set; }
        public decimal Balancing { get; set; }
    }
}
