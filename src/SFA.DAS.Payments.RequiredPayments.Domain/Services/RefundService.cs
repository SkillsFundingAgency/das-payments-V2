using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class RefundService : IRefundService
    {
        public List<RequiredPayment> GetRefund(decimal amount, List<Payment> paymentHistory)
        {
            if (amount >= 0)
            {
                return new List<RequiredPayment>();
            }

            var totalPaidInHistory = paymentHistory.Sum(x => x.Amount);
            if (totalPaidInHistory <= 0)
            {
                return new List<RequiredPayment>();
            }

            var totalRefundPercent = amount / totalPaidInHistory; // will be negative
            totalRefundPercent = Math.Max(totalRefundPercent, -1);

            return paymentHistory.GroupBy(x => new
                {
                    SfaContributionPercentage = x.SfaContributionPercentage,
                    EarningType = ToEarningType(x.FundingSource),
                    PriceEpisodeIdentifier = x.PriceEpisodeIdentifier,
                    AccountId = x.AccountId,
                    TransferSenderAccountId = x.TransferSenderAccountId,
                    TransactionType = x.TransactionType
            })
                .Select(group =>
                {
                    var amountForGroup = group.Sum(x => x.Amount);
                    return new RequiredPayment
                    {
                        Amount = (amountForGroup * totalRefundPercent).AsRounded(),
                        EarningType = group.Key.EarningType,
                        SfaContributionPercentage = group.Key.SfaContributionPercentage,
                        PriceEpisodeIdentifier = group.Key.PriceEpisodeIdentifier,
                        AccountId = group.Key.AccountId,
                        TransferSenderAccountId = group.Key.TransferSenderAccountId,
                        TransactionType = group.Key.TransactionType
                    };
                })
                .Where(x => x.Amount < 0)
                .ToList();
        }

        EarningType ToEarningType(FundingSourceType fundingSource)
        {
            switch (fundingSource)
            {
                case FundingSourceType.Transfer:
                case FundingSourceType.Levy:
                    return EarningType.Levy;
                case FundingSourceType.CoInvestedEmployer:
                case FundingSourceType.CoInvestedSfa:
                    return EarningType.CoInvested;
                case FundingSourceType.FullyFundedSfa:
                    return EarningType.Incentive;
            }

            throw new NotImplementedException($"Unknown funding source: {fundingSource}");
        }
    }
}
