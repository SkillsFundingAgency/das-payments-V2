using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public abstract class RequiredPaymentEvent : PeriodisedPaymentEvent, IRequiredPaymentEvent
    {
        public Guid PaymentsDueEventId { get; set; }
    }
}