using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application
{
    public interface ICoInvestedFundingSourcePaymentEventMapper
    {
        CoInvestedFundingSourcePaymentEvent MapTo(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent, Payment payment);

        CoInvestedPayment MapFrom(ApprenticeshipContractType2RequiredPaymentEvent requiredPaymentsEvent);

    }



}