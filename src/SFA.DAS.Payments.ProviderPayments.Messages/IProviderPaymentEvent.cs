using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public interface IProviderPaymentEvent : IPeriodisedPaymentEvent
    {
        TransactionType TransactionType { get; }
        FundingSourceType FundingSourceType { get; }  //Backwards Compatibility
        decimal SfaContributionPercentage { get; }
    }
}