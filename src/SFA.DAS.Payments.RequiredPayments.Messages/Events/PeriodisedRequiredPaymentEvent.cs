using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class PeriodisedRequiredPaymentEvent : PeriodisedPaymentEvent, IPeriodisedRequiredPaymentEvent
    {
        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(PeriodisedRequiredPaymentEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(PeriodisedRequiredPaymentEvent)))
                       .ToArray());
        }
    }
}