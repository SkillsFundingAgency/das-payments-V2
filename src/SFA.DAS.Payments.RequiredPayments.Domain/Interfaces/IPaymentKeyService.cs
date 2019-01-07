using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IPaymentKeyService
    {
        string GeneratePaymentKey(string learnAimReference, int transactionType, DeliveryPeriod deliveryPeriod);
    }
}