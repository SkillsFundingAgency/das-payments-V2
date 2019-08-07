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
                AccountName = source.AccountName,
                Balance = source.Balance,
                IsLevyPayer = source.IsLevyPayer,
                TransferAllowance = source.TransferAllowance
            };
        }
    }
}
