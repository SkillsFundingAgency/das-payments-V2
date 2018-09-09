using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class RequiredPaymentEvent : IRequiredPayment
    {
        public DateTimeOffset EventTime { get; set; }
        public string JobId { get; set; }
        public long Ukprn { get;set; }
        public Learner Learner { get;set; }
        public LearningAim LearningAim { get; set;}
        public byte Period { get; set;}
        public string PriceEpisodeIdentifier { get; set;}
        public decimal AmountDue { get; set; }
        public NamedCalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
    }
}