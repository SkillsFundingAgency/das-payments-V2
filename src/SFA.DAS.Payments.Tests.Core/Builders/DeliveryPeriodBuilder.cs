using System;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class DeliveryPeriodBuilder 
    {
        protected DateTime Date { get; set; } = DateTime.MinValue;
        protected int Year { get; set; } = -1;
        protected int Month { get; set; } = -1;

        public byte BuildLastOnProgPeriod()
        {
            var lastDayOfMonth = DateTime.DaysInMonth(Date.Year, Date.Month);
            if (Date.Day < lastDayOfMonth)
            {
                var newDate = Date.AddMonths(-1);
                Date = newDate;
            }

            return Build();
        }

        public byte Build()
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
                Date = new DateTime(Year, Month, 1);
            }

            if (Date.Month < 8)
            {
                return (byte)(Date.Month + 5);
            }
            return (byte)(Date.Month - 7);
        }

        protected DeliveryPeriodBuilderType BuilderType { get; set; }
        protected DatePortionsIncluded IncludedPortions { get; set; }
        
        public virtual DeliveryPeriodBuilder WithSpecDate(string date)
        {
            return WithDate(date.ToDate());
        }

        public DeliveryPeriodBuilder WithDate(DateTime date)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDateComponents)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDate;

            Date = date;
            Year = date.Year;
            Month = date.Month;
            return this;
        }

        public DeliveryPeriodBuilder WithYear(int year)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDate)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDateComponents;
            IncludedPortions |= DatePortionsIncluded.Year;
            Year = year;

            return this;
        }

        public DeliveryPeriodBuilder WithMonth(int month)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDate)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDateComponents;
            IncludedPortions |= DatePortionsIncluded.Month;
            Month = month;

            return this;
        }

        protected enum DeliveryPeriodBuilderType
        {
            None,
            WithDate,
            WithDateComponents,
        }

        [Flags]
        protected enum DatePortionsIncluded
        {
            None,
            Year,
            Month,
        }
    }
}