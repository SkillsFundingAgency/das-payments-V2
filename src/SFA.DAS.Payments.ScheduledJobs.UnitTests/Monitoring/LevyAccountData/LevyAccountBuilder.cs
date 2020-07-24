using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.UnitTests.Monitoring.LevyAccountData
{
    public class LevyAccountBuilder
    {
        private readonly LevyAccountModel levyAccountModel = new LevyAccountModel
        {
            Balance = 100,
            TransferAllowance = 100,
            IsLevyPayer = true
        };
        
        public LevyAccountBuilder SetBalance(decimal balance)
        {
            levyAccountModel.Balance = balance;
            return this;
        }
        
        public LevyAccountBuilder SetTransferAllowance(decimal transferAllowance)
        {
            levyAccountModel.TransferAllowance = transferAllowance;
            return this;
        }

        public LevyAccountBuilder SetIsLevyPayer(bool isLevyPayer)
        {
            levyAccountModel.IsLevyPayer = isLevyPayer;
            return this;
        }
        
        public IList<LevyAccountModel> Build(int count)
        {
            return Enumerable.Range(0, count).Select(index => new LevyAccountModel
            {
                AccountId = index + 1,
                Balance = levyAccountModel.Balance,
                TransferAllowance = levyAccountModel.TransferAllowance,
                IsLevyPayer = levyAccountModel.IsLevyPayer,
                AccountName = "AccountName",
            }).ToList();
        }
    }
}
