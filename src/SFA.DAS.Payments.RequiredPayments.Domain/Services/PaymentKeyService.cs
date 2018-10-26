using System.Globalization;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class PaymentKeyService : IPaymentKeyService
    {
        public string GeneratePaymentKey(string priceEpisodeIdentifier, string learnAimReference, int transactionType, CalendarPeriod deliveryPeriod)
        {
            return string.Join("-",
                new[]
                {
                    priceEpisodeIdentifier.ToLowerInvariant(),
                    learnAimReference.ToLowerInvariant(),
                    transactionType.ToString(CultureInfo.InvariantCulture),
                    deliveryPeriod.Name
                }
            );
        }
    }
}