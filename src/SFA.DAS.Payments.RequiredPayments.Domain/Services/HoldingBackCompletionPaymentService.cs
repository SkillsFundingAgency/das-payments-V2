using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class HoldingBackCompletionPaymentService : IHoldingBackCompletionPaymentService
    {
        public bool HoldBackCompletionPayment(decimal employerPayments, PriceEpisode priceEpisode)
        {
            var employerContribution = priceEpisode.EmployerContribution ?? 0;
            var completionHoldBackExemptionCode = priceEpisode.CompletionHoldBackExemptionCode ?? 0;

            if (completionHoldBackExemptionCode > 0)
                return false;

            return employerContribution < employerPayments;
        }
    }
}