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

            if (fundingSourcePayments.Select(p => p.AmountDue).Sum() < requiredPayment.AmountDue)
                fundingSourcePayments.AddRange(coInvestedPaymentProcessor.Process(requiredPayment));

            return fundingSourcePayments;
        }
    }
}
