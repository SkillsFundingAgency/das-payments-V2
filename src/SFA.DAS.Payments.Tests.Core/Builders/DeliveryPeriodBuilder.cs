using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class DeliveryPeriodBuilder : CalendarPeriodBuilder<DeliveryPeriodBuilder, DeliveryPeriod>
    {
        public DeliveryPeriod BuildLastOnProgPeriod()
        {
            var lastDayOfMonth = DateTime.DaysInMonth(Date.Year, Date.Month);
            if (Date.Day < lastDayOfMonth)
            {
                var newDate = Date.AddMonths(-1);
                Date = newDate;
            }

            return Build();
        }

        public DeliveryPeriod Build()
        {
            var builtInstance = BuildInstance();
            builtInstance.Identifier = $"{builtInstance.Year}-{builtInstance.Month:D2}";
            return builtInstance;
        }
    }
}