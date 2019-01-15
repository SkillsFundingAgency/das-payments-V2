using System.Globalization;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class PaymentKeyService : IPaymentKeyService
    {
        public string GeneratePaymentKey(string learnAimReference, int transactionType, short academicYear, byte deliveryPeriod)
        {
            return string.Join("~",
                new[]
                {
                    learnAimReference.ToLowerInvariant(),
                    transactionType.ToString(CultureInfo.InvariantCulture),
                    academicYear.ToString(),
                    deliveryPeriod.ToString(),
                }
            );
        }
    }
}