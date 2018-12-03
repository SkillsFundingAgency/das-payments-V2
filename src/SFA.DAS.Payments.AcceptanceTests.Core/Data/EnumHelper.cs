using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public static class EnumHelper
    {
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