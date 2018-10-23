using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public abstract class RequiredPaymentEvent : IRequiredPayment
    {
        public DateTimeOffset EventTime { get; set; }
        public string JobId { get; set; }
        public long Ukprn { get;set; }
        public Learner Learner { get;set; }
        public LearningAim LearningAim { get; set;}
        public string PriceEpisodeIdentifier { get; set;}
        public decimal AmountDue { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
}