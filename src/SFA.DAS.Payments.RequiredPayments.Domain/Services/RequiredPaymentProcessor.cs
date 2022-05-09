using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RequiredPaymentProcessor : IRequiredPaymentProcessor
    {
        private readonly IPaymentDueProcessor paymentsDueProcessor;
        private readonly IRefundService refundService;

        public RequiredPaymentProcessor(IPaymentDueProcessor paymentsDueProcessor, IRefundService refundService)
        {
            this.paymentsDueProcessor = paymentsDueProcessor;
            this.refundService = refundService;
        }

        public List<RequiredPayment> GetRequiredPayments(Earning earning, List<Payment> paymentHistory)
        {
            var result = new List<RequiredPayment>();

            if (earning.EarningType != EarningType.Incentive && earning.SfaContributionPercentage.HasValue)
                result.AddRange(RefundPaymentsWithDifferentSfaContribution(earning.SfaContributionPercentage.Value, paymentHistory));

            var validPaymentHistory = paymentHistory
                .Where(p => earning.EarningType == EarningType.Incentive ||
                            !earning.SfaContributionPercentage.HasValue ||
                            p.SfaContributionPercentage == earning.SfaContributionPercentage)
                .ToList();

            result.AddRange(GenerateRefundsForPreviousEmployers(earning, validPaymentHistory));

            var currentEmployerPaymentHistory = validPaymentHistory
                .Where(x => x.AccountId == earning.AccountId)
                .ToList();

            var currentEmployerRequiredPaymentAmount = paymentsDueProcessor.CalculateRequiredPaymentAmount(earning.Amount, currentEmployerPaymentHistory);

            if (currentEmployerRequiredPaymentAmount < 0)
            {
                result.AddRange(refundService.GetRefund(currentEmployerRequiredPaymentAmount, validPaymentHistory));
                return result;
            }

            if (currentEmployerRequiredPaymentAmount == 0)
            {
                return result;
            }

            if (!earning.SfaContributionPercentage.HasValue)
            {
                throw new ArgumentException("Trying to use a null SFA Contribution % for a positive earning");
            }

            result.Add(new RequiredPayment
            {
                Amount = currentEmployerRequiredPaymentAmount,
                EarningType = earning.EarningType,
                SfaContributionPercentage = earning.SfaContributionPercentage.Value,
                PriceEpisodeIdentifier = earning.PriceEpisodeIdentifier,
                AccountId = earning.AccountId,
                TransferSenderAccountId = earning.TransferSenderAccountId
            });

            return result;
        }

        private List<RequiredPayment> GenerateRefundsForPreviousEmployers(Earning earning, List<Payment> paymentHistory)
        {
            var previousEmployersPaymentHistory = paymentHistory
                .Where(x => x.AccountId != earning.AccountId)
                .ToList();

            if (previousEmployersPaymentHistory.Any())
            {
                var previousEmployersRequiredPaymentAmount = paymentsDueProcessor.CalculateRequiredPaymentAmount(0m, previousEmployersPaymentHistory);

                if (previousEmployersRequiredPaymentAmount < 0) return refundService.GetRefund(previousEmployersRequiredPaymentAmount, previousEmployersPaymentHistory);
            }

            return new List<RequiredPayment>();
        }

        private List<RequiredPayment> RefundPaymentsWithDifferentSfaContribution(decimal currentSfaContributionPercentage, List<Payment> paymentHistory)
        {
            return paymentHistory
                .Where(payment => payment.SfaContributionPercentage != currentSfaContributionPercentage)
                .GroupBy(payment => payment.SfaContributionPercentage)
                .SelectMany(group => refundService.GetRefund(-1 * group.Sum(x => x.Amount), group.ToList()))
                .ToList();
        }
    }
}