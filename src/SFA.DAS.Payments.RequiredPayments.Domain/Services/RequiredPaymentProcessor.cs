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

            var previousEmployersPaymentHistory = paymentHistory
                .Where(x => x.AccountId != earning.AccountId)
                .ToList();

            result.AddRange(GenerateRequiredPaymentForEmployer(earning, 0m, previousEmployersPaymentHistory));

            var currentEmployerPaymentHistory = validPaymentHistory
                .Where(x => x.AccountId == earning.AccountId)
                .ToList();

            result.AddRange(GenerateRequiredPaymentForEmployer(earning, earning.Amount, currentEmployerPaymentHistory));

            return result;
        }

        private List<RequiredPayment> GenerateRequiredPaymentForEmployer(Earning earning, decimal amountDue, List<Payment> employersPaymentHistory)
        {
            var employersRequiredPaymentAmount = paymentsDueProcessor.CalculateRequiredPaymentAmount(amountDue, employersPaymentHistory);

            if (employersRequiredPaymentAmount == 0) return new List<RequiredPayment>();

            if (employersRequiredPaymentAmount < 0) return refundService.GetRefund(employersRequiredPaymentAmount, employersPaymentHistory);

            if (!earning.SfaContributionPercentage.HasValue) throw new ArgumentException($"Trying to use a null SFA Contribution % for a positive earning, Earning Details: Amount: {employersRequiredPaymentAmount}, EarningType : {earning.EarningType}, PriceEpisodeIdentifier : {earning.PriceEpisodeIdentifier}");

            return new List<RequiredPayment> { new RequiredPayment
            {
                Amount = employersRequiredPaymentAmount,
                EarningType = earning.EarningType,
                SfaContributionPercentage = earning.SfaContributionPercentage.Value,
                PriceEpisodeIdentifier = earning.PriceEpisodeIdentifier,
                AccountId = earning.AccountId,
                TransferSenderAccountId = earning.TransferSenderAccountId
            }};
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