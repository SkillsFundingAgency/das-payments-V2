using System;
using System.Linq;
using System.Runtime.Serialization;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    [KnownType("GetInheritors")]
    public abstract class RequiredPaymentEvent : PaymentsEvent,IRequiredPayment
    {
        private static Type[] inheritors;

        public string PriceEpisodeIdentifier { get; set; }
        public decimal AmountDue { get; set; }
        public CalendarPeriod DeliveryPeriod { get; set; }

        private static Type[] GetInheritors()
        {
            return inheritors ?? (inheritors = typeof(RequiredPaymentEvent).Assembly.GetTypes()
                       .Where(x => x.IsSubclassOf(typeof(RequiredPaymentEvent)))
                       .ToArray());
        }
    }
}