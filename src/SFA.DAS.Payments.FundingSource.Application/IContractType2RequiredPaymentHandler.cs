using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.FundingSource.Application
{
    public interface IContractType2RequiredPaymentHandler
    {
        IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(ApprenticeshipContractType2RequiredPaymentEvent message);
    }
}