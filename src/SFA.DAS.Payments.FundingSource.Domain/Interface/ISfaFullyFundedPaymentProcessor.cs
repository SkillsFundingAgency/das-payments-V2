using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Interface
{
    public interface ISfaFullyFundedPaymentProcessor
    {
        FundingSourcePayment CalculatePaymentAmount(decimal amount);
    }
}