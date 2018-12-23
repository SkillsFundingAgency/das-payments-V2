using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public static class EnumHelper
    {
        private static readonly HashSet<TransactionType> OnProgTypes = new HashSet<TransactionType>(Enum.GetValues(typeof(OnProgrammeEarningType)).Cast<int>().Select(t => (TransactionType) t));
        private static readonly HashSet<TransactionType> IncentiveTypes = new HashSet<TransactionType>(Enum.GetValues(typeof(IncentiveEarningType)).Cast<int>().Select(t => (TransactionType)t));
        private static readonly HashSet<TransactionType> FunctionalSkillTypes = new HashSet<TransactionType>(Enum.GetValues(typeof(FunctionalSkillType)).Cast<int>().Select(t => (TransactionType)t));

        public static bool IsOnProgType(TransactionType type)
        {
            return OnProgTypes.Contains(type);
        }

        public static bool IsIncentiveType(TransactionType type)
        {
            return IncentiveTypes.Contains(type);
        }

        public static bool IsFunctionalSkillType(TransactionType type)
        {
            return FunctionalSkillTypes.Contains(type);
        }

        public static TransactionType ToTransactionType(string transactionType)
        {
            transactionType = transactionType.ToLower();

            if (transactionType.Contains("learning")) return TransactionType.Learning;
            if (transactionType.Contains("completion")) return TransactionType.Completion;
            if (transactionType.Contains("balancing")) return TransactionType.Balancing;
            if (transactionType.Contains("first16to18employerincentive")) return TransactionType.First16To18EmployerIncentive;
            if (transactionType.Contains("learningSupport")) return TransactionType.LearningSupport;

            throw new InvalidOperationException($"Unknown transaction type: '{transactionType}'");
        }

        public static FundingSourceType ToFundingSourceType(string fundingSourceType)
        {
            fundingSourceType = fundingSourceType.ToLower();

            if (fundingSourceType.Contains("coinvestedsfa")) return FundingSourceType.CoInvestedSfa;
            if (fundingSourceType.Contains("coinvestedemployer")) return FundingSourceType.CoInvestedEmployer;
            if (fundingSourceType.Contains("fullyfundedsfa")) return FundingSourceType.FullyFundedSfa;
            if (fundingSourceType.Contains("levy")) return FundingSourceType.Levy;
            if (fundingSourceType.Contains("transfer")) return FundingSourceType.Transfer;

             throw new InvalidOperationException($"Invalid funding source: '{fundingSourceType}'");
        }

        public static string ToAttributeName(this TransactionType transactionType)
        {
            switch (transactionType)
            {
                case TransactionType.Learning:
                    return "PriceEpisodeOnProgPayment";
                case TransactionType.Balancing:
                    return "PriceEpisodeBalancePayment";
                case TransactionType.Completion:
                    return "PriceEpisodeCompletionPayment";
                case TransactionType.LearningSupport:
                    return "PriceEpisodeLSFCash";
                case TransactionType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case TransactionType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case TransactionType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case TransactionType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case TransactionType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case TransactionType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case TransactionType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case TransactionType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case TransactionType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case TransactionType.OnProgrammeMathsAndEnglish:
                    return "MathEngOnProgPayment";
                case TransactionType.BalancingMathsAndEnglish:
                    return "MathEngBalPayment";

                default:
                    throw new NotImplementedException($"Cannot get FM36 attribute name.  Unhandled transaction type: {transactionType}");
            }
        }
    }
}