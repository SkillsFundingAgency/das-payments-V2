using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application
{
    public interface ICoInvestedFundingSourcePaymentEventMapper
    {
        CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, CoInvestedPayment payment);
        CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, SfaCoInvestedPayment payment);
        CoInvestedFundingSourcePaymentEvent MapToCoInvestedPaymentEvent(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, EmployerCoInvestedPayment payment);
        RequiredCoInvestedPayment MapToRequiredCoInvestedPayment(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent);
    }

}