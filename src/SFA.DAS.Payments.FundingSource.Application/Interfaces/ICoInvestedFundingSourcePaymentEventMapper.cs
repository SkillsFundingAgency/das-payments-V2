using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ICoInvestedFundingSourcePaymentEventMapper
    {
        CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, FundingSourcePayment payment);
        RequiredCoInvestedPayment MapToRequiredCoInvestedPayment(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent);
    }
}