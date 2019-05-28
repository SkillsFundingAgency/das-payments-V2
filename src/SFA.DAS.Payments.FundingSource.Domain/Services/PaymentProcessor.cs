using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.FundingSource.Domain.Interface;
using SFA.DAS.Payments.FundingSource.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Services
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly ILevyPaymentProcessor levyPaymentProcessor;
        private readonly ICoInvestedPaymentProcessor coInvestedPaymentProcessor;
        private readonly ITransferPaymentProcessor transferPaymentProcessor;

        public PaymentProcessor(ILevyPaymentProcessor levyPaymentProcessor, ICoInvestedPaymentProcessor coInvestedPaymentProcessor, ITransferPaymentProcessor transferPaymentProcessor)
        {
            this.levyPaymentProcessor = levyPaymentProcessor;
            this.coInvestedPaymentProcessor = coInvestedPaymentProcessor;
            this.transferPaymentProcessor = transferPaymentProcessor ?? throw new ArgumentNullException(nameof(transferPaymentProcessor));
        }

        public IReadOnlyList<FundingSourcePayment> Process(RequiredPayment requiredPayment)
        {
            var fundingSourcePayments = new List<FundingSourcePayment>();
            if (requiredPayment.IsTransfer)
            {
                fundingSourcePayments.AddRange(transferPaymentProcessor.Process(requiredPayment));
                var remainingTransferAmount = requiredPayment.AmountDue - fundingSourcePayments.Sum(transfer => transfer.AmountDue);
                if (remainingTransferAmount > 0)
                    fundingSourcePayments.Add(new UnableToFundTransferPayment
                    {
                        AmountDue = remainingTransferAmount,
                        Type = FundingSourceType.Transfer
                    });
                return fundingSourcePayments;
            }

            fundingSourcePayments.AddRange(levyPaymentProcessor.Process(requiredPayment));

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
