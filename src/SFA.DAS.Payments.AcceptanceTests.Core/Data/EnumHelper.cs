using System;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public static class EnumHelper
    {
        private static readonly TransactionType[] onProgTypes = { TransactionType.Learning, TransactionType.Balancing, TransactionType.Completion };
        private static readonly TransactionType[] incentiveTypes = { TransactionType.First16To18EmployerIncentive, TransactionType.First16To18ProviderIncentive, TransactionType.Second16To18EmployerIncentive, TransactionType.Second16To18ProviderIncentive };
        private static readonly TransactionType[] functionalSkillTypes = { TransactionType.OnProgrammeMathsAndEnglish, TransactionType.BalancingMathsAndEnglish };

        public static bool IsOnProgType(TransactionType type)
        {
            return onProgTypes.Contains(type);
        }

        public static bool IsIncentiveType(TransactionType type)
        {
            return incentiveTypes.Contains(type);
        }

        public static bool IsFunctionalSkillType(TransactionType type)
        {
            return functionalSkillTypes.Contains(type);
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
                default:
                    throw new NotImplementedException($"Cannot get FM36 attribute name.  Unhandled transaction type: {transactionType}");
            }
        }
    }
}