using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.RequiredPayments.Domain
{
    public interface IHoldingBackCompletionPaymentService
    {
        bool ShouldHoldBackCompletionPayment(decimal expectedContribution, PriceEpisode priceEpisode);
    }
}
