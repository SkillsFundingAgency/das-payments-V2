using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class RequiredPaymentEvent : PeriodisedPaymentEvent, IRequiredPaymentEvent
    {
        private static Type[] inheritors;
        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(RequiredPaymentEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(RequiredPaymentEvent)))
                       .ToArray());
        }
    }
}