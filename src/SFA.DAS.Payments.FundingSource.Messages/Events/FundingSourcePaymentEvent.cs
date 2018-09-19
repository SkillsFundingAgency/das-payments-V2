using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Messages.Events
{
    public abstract class FundingSourcePaymentEvent : IPaymentsEvent
    {
        public string PriceEpisodeIdentifier { get; set; }

        public decimal AmountDue { get; set; }

        public NamedCalendarPeriod CollectionPeriod { get; set; }

        public CalendarPeriod DeliveryPeriod { get; set; }

        public DateTimeOffset EventTime { get; set; }

        public long Ukprn { get; set; }

        public Learner Learner { get; set; }

        public LearningAim LearningAim { get; set; }

        public string JobId { get; set; }

        public byte Period { get; set; }

        public byte ContractType { get; set; }
    }
}