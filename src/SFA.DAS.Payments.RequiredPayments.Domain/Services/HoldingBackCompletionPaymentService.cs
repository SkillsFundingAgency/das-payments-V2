using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class HoldingBackCompletionPaymentService : IHoldingBackCompletionPaymentService
    {
        public bool HoldBackCompletionPayment(decimal employerPayments, PriceEpisode priceEpisode)
        {
            if (!priceEpisode.EmployerContribution.HasValue)
                return false;

            if (!priceEpisode.CompletionHoldBackExemptionCode.HasValue)
                return false;

            if (priceEpisode.CompletionHoldBackExemptionCode.Value > 0)
                return false;

            return employerPayments < priceEpisode.EmployerContribution.Value;
        }
    }
}