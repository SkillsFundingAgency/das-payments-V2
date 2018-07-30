using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Events
{
    public interface IPaymentsDueEvent : IPaymentsEvent
    {
        PaymentDue PaymentDue { get; set; }
    }
}