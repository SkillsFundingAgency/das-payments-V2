using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ISfaFullyFundedFundingSourcePaymentEventMapper
    {
        SfaFullyFundedFundingSourcePaymentEvent Map(IncentiveRequiredPayment requiredPaymentsEvent, FundingSourcePayment payment);
    }
}