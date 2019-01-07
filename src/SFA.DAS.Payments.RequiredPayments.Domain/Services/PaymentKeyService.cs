using System.Globalization;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class PaymentKeyService : IPaymentKeyService
    {
        public string GeneratePaymentKey(string learnAimReference, int transactionType, DeliveryPeriod deliveryPeriod)
        {
            return string.Join("~",
                new[]
                {
                    learnAimReference.ToLowerInvariant(),
                    transactionType.ToString(CultureInfo.InvariantCulture),
                    deliveryPeriod.Identifier,
                }
            );
        }
    }
}