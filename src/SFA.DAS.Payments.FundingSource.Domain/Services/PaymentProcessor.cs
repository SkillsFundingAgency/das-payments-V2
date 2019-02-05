using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILevyPaymentProcessor levyPaymentProcessor;
        private readonly ICoInvestedPaymentProcessor coInvestedPaymentProcessor;

        public PaymentProcessor(ILevyPaymentProcessor levyPaymentProcessor, ICoInvestedPaymentProcessor coInvestedPaymentProcessor)
        {
            this.levyPaymentProcessor = levyPaymentProcessor;
            this.coInvestedPaymentProcessor = coInvestedPaymentProcessor;
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            var fundingSourcePayments = new List<FundingSourcePayment>(levyPaymentProcessor.Process(requiredPayment));

            var amountDue = requiredPayment.AmountDue - fundingSourcePayments.Select(p => p.AmountDue).Sum();
            if (amountDue != 0m)
            {
                var partFundedRequiredPayment = new RequiredPayment
                {
                    AmountDue = amountDue, 
                    SfaContributionPercentage = requiredPayment.SfaContributionPercentage
                };

                fundingSourcePayments.AddRange(coInvestedPaymentProcessor.Process(partFundedRequiredPayment));
            }

            return fundingSourcePayments;
        }
    }
}
