﻿using System;
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
            Year = date.Year;
            Month = date.Month;
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
