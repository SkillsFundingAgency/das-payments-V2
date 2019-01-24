using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ProviderPayments.Messages
{
    public interface IProviderPaymentEvent : IPeriodisedPaymentEvent
    {
        byte ContractType { get;  } //Backwards Compatibility //TODO: we will eventually have events: ACT1Transfer, ACT1Levy, ACT1EmployerCoInvested, ACT1SfaCoInvested, ACT2EmployerCoInvested, etc
        TransactionType TransactionType { get; }
        FundingSourceType FundingSourceType { get; }  //Backwards Compatibility
        decimal SfaContributionPercentage { get; }
    }
}