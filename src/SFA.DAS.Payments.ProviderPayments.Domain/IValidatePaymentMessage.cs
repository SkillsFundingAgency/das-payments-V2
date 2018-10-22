using SFA.DAS.Payments.ProviderPayments.Domain.Models;

namespace SFA.DAS.Payments.ProviderPayments.Domain
{
    public interface IValidatePaymentMessage
    {
        bool IsLatestIlrPayment(PaymentMessageValidationRequest request);
    }
}