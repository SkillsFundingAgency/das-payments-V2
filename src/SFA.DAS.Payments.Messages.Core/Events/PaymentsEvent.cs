using SFA.DAS.Payments.Model.Core;
using System;

namespace SFA.DAS.Payments.Messages.Core.Events
{
    public class PaymentsEvent : IPaymentsEvent
    {
        public long JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get; set; }
        public Learner Learner { get; set; }
        public LearningAim LearningAim { get; set; }
        public DateTime IlrSubmissionDateTime { get; set; }
        public CalendarPeriod CollectionPeriod { get; set; }
    }
}
