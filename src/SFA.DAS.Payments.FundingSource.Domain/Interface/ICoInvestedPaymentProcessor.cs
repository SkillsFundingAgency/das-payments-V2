using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ICoInvestedPaymentProcessor
    {
        CoInvestedFundingSourcePaymentEvent Process(ApprenticeshipContractType2RequiredPaymentEvent message);
    }
}