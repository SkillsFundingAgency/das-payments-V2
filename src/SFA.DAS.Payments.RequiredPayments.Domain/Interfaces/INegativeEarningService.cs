using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface INegativeEarningService
    {
        List<RequiredPayment> ProcessNegativeEarning(decimal amount, List<Payment> paymentHistory, int deliveryPeriod, string priceEpisodeIdentifier);
    }
}
