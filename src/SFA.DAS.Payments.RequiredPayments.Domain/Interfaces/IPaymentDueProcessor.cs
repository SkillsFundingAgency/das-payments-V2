using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IPaymentDueProcessor
    {
        RequiredPaymentEvent ProcessPaymentDue(PaymentDueEvent paymentDue, Payment[] paymentHistory);
    }
}