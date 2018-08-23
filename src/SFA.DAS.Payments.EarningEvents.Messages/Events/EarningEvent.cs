using System;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public abstract class EarningEvent: IEarningEvent
    {
        public string JobId { get; set; }
        public DateTimeOffset EventTime { get; set; }
        public long Ukprn { get;set; }
        public Learner Learner { get;set; }
        public LearningAim LearningAim { get; set;}
    }
}