using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public class PaymentDueEvent : IPaymentDueEvent
    {
        public long Ukprn { get; set; }

        public string JobId { get; set; }
        
        public Learner Learner {get; set; }

        public LearningAim LearningAim { get; set; }
        public string PriceEpisodeIdentifier { get; set; }

        public decimal Amount { get; set; }

        public NamedCalendarPeriod CollectionPeriod { get; set; }

        public NamedCalendarPeriod DeliveryPeriod { get; set; }

        public int TransactionType { get; set; }
    }
}
