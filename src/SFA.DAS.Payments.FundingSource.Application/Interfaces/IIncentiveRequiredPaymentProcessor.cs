using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IIncentiveRequiredPaymentProcessor
    {
        SfaFullyFundedFundingSourcePaymentEvent Process(IncentiveRequiredPaymentEvent requiredPayment);
    }
}