using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IContractType1RequiredPaymentEventFundingSourceService
    {
        Task EnrolRequiredPayment(ApprenticeshipContractType1RequiredPaymentEvent paymentEvent);
        Task<IReadOnlyList<FundingSourcePaymentEvent>> GetFundedPayments(long employerAccountId);
    }
}