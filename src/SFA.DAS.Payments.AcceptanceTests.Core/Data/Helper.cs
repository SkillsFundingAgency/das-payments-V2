using System;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Helper
    {
        public static TransactionType GetTransactionType(string transactionType)
        {
            transactionType = transactionType.ToLower();
            return transactionType.Contains("learning")
                ? Model.Core.Entities.TransactionType.Learning
                : transactionType.Contains("completion")
                    ? Model.Core.Entities.TransactionType.Completion
                    : transactionType.Contains("balancing")
                        ? Model.Core.Entities.TransactionType.Balancing
                        : transactionType.Contains("first16to18employerincentive")
                            ? Model.Core.Entities.TransactionType.First16To18EmployerIncentive
                            : throw new InvalidOperationException($"Unknown transaction type: '{transactionType}'");
        }

        public static FundingSourceType GetFundingSourceType(string fundingSourceType)
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