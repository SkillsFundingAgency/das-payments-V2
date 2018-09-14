using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IAct2PaymentDueProcessor
    {
        ApprenticeshipContractType2RequiredPaymentEvent ProcessPaymentDue(ApprenticeshipContractType2PaymentDueEvent paymentDue, Payment[] paymentHistory);
    }
}