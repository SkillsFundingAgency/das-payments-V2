using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class DeliveryPeriodBuilder 
    {
        protected DateTime Date { get; set; } = DateTime.MinValue;
        protected int Year { get; set; } = -1;
        protected int Month { get; set; } = -1;

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
                Date = new DateTime(Year, Month, 1);
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