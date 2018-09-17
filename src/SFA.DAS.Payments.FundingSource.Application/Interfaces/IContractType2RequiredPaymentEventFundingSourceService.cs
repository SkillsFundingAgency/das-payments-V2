using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;
using blah.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IContractType2RequiredPaymentEventFundingSourceService
    {
        IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(ApprenticeshipContractType2RequiredPaymentEvent message);
    }
}