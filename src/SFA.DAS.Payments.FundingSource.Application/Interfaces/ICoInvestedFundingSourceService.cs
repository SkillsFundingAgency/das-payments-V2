using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using System.Collections.Generic;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ICoInvestedFundingSourceService
    {
        IEnumerable<CoInvestedFundingSourcePaymentEvent> GetFundedPayments(CalculatedRequiredCoInvestedAmount message);
    }
}