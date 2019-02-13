using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class SfaFullyFundedPaymentProcessor: ISfaFullyFundedPaymentProcessor
    {
        public FundingSourcePayment CalculatePaymentAmount(decimal amount)
        {
            return new SfaCoInvestedPayment()
            {
                Type = FundingSourceType.FullyFundedSfa,
                AmountDue = amount.AsRounded()
            };
        }
    }
}