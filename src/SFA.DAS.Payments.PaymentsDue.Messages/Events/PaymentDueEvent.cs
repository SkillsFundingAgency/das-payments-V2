using System;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public abstract class PaymentDueEvent : PeriodisedPaymentEvent, IPaymentDueEvent
    {
    }
}
