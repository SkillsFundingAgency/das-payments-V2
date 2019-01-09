using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.Tests.Core.Builders
{
    public class CalendarPeriodBuilder
    {
        protected DateTime Date { get; set; } = DateTime.MinValue;
        protected int Year { get; set; } = -1;
        protected int Month { get; set; } = -1;
        protected int Day { get; set; } = 1;
    }

    public class CalendarPeriodBuilder<TBuilder, T> : CalendarPeriodBuilder
        where T : SfaPeriodBaseClass, new()
        where TBuilder : CalendarPeriodBuilder<TBuilder, T>
    {
        protected DeliveryPeriodBuilderType BuilderType { get; set; }
        protected DatePortionsIncluded IncludedPortions { get; set; }

        protected T BuildInstance()
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

            var instance = new T();
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

        public virtual TBuilder WithSpecDate(string date)
        {
            return WithDate(date.ToDate());
        }

        public TBuilder WithDate(DateTime date)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDateComponents)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDate;

            Date = date;
            return (TBuilder) this;
        }

        public TBuilder WithYear(int year)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDate)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDateComponents;
            IncludedPortions |= DatePortionsIncluded.Year;
            Year = year;

            return (TBuilder)this;
        }

        public TBuilder WithMonth(int month)
        {
            if (BuilderType == DeliveryPeriodBuilderType.WithDate)
            {
                throw new ArgumentException("Please either use a date or date components but not both");
            }

            BuilderType = DeliveryPeriodBuilderType.WithDateComponents;
            IncludedPortions |= DatePortionsIncluded.Month;
            Month = month;

            return (TBuilder)this;
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
            Day,
        }
    }
}
