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

        protected DeliveryPeriod BuildInstance()
        {
            if (BuilderType == DeliveryPeriodBuilderType.None)
            {
                throw new ArgumentException("Please include either a date or date components to build");
            }

            if (BuilderType == DeliveryPeriodBuilderType.WithDateComponents)
            {
                if (!IncludedPortions.HasFlag(DatePortionsIncluded.Year) ||
                    !IncludedPortions.HasFlag(DatePortionsIncluded.Month))
                {
                    throw new ArgumentException("Please include at least a month and a year");
                }
                Date = new DateTime(Year, Month, Day);
            }

            var instance = new DeliveryPeriod();
            instance.Month = (byte)Date.Month;
            instance.Year = (short)Date.Year;
            if (instance.Month < 8)
            {
                instance.Period = (byte)(instance.Month + 5);
            }
            else
            {
                instance.Period = (byte)(instance.Month - 7);
            }

            return instance;
        }

        public DeliveryPeriod Build()
        {
            var builtInstance = BuildInstance();
            builtInstance.Identifier = $"{builtInstance.Year}-{builtInstance.Month:D2}";
            return builtInstance;
        }
    }
}