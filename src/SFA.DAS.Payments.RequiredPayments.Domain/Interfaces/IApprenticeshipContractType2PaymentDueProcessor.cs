using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IApprenticeshipContractType2PaymentDueProcessor
    {
        decimal CalculateRequiredPaymentAmount(decimal amountDue, Payment[] paymentHistory);
    }
}