using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class PaymentDueEvent : PeriodisedPaymentEvent, IPaymentDueEvent
    {
        private static Type[] inheritors;
        public Guid EarningEventId { get; set; }
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(PaymentDueEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(PaymentDueEvent)))
                       .ToArray());
        }
    }

}