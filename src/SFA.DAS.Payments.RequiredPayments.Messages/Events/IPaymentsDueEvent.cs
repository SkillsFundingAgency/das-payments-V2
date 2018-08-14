using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public interface IPaymentsDueEvent : IPaymentsEvent
    {
        PaymentDueEntity PaymentDueEntity { get; set; }
    }
}