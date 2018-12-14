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

            return transactionType.Contains("learning")
                ? TransactionType.Learning
                : transactionType.Contains("completion")
                    ? TransactionType.Completion
                    : transactionType.Contains("balancing")
                        ? TransactionType.Balancing
                        : transactionType.Contains("first16to18employerincentive")
                            ? TransactionType.First16To18EmployerIncentive
                            : throw new InvalidOperationException($"Unknown transaction type: '{transactionType}'");
        }

        public static FundingSourceType ToFundingSourceType(string fundingSourceType)
        {
            fundingSourceType = fundingSourceType.ToLower();
            return fundingSourceType.Contains("coinvestedsfa")
                ? FundingSourceType.CoInvestedSfa
                : fundingSourceType.Contains("coinvestedemployer")
                    ? FundingSourceType.CoInvestedEmployer
                    : fundingSourceType.Contains("fullyfundedsfa")
                        ? FundingSourceType.FullyFundedSfa
                        : fundingSourceType.Contains("levy")
                            ? FundingSourceType.Levy
                            : fundingSourceType.Contains("transfer")
                                ? FundingSourceType.Transfer
                                : throw new InvalidOperationException($"Invalid funding source: '{fundingSourceType}'");
        }
    }
}