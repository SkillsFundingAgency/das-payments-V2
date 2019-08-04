using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public static class EmployerExtensions
    {
        public static LevyAccountModel ToModel(this Employer source)
        {
            return new LevyAccountModel
            {
                AccountId = source.AccountId,
                AccountHashId = source.AccountHashId,
                AccountName = source.AccountName,
                Balance = source.Balance,
                SequenceId = source.SequenceId,
                IsLevyPayer = source.IsLevyPayer,
                TransferAllowance = source.TransferAllowance
            };
        }
    }
}
