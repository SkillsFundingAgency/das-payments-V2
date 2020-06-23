using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.ScheduledJobs.Monitoring.LevyAccountData
{
    public class CombinedLevyAccountsDto
    {
        private readonly List<LevyAccountModel> dasLevyAccountDetails;
        private readonly List<LevyAccountModel> paymentsLevyAccountDetails;

        public CombinedLevyAccountsDto(IList<LevyAccountModel> dasLevyAccountDetails, IList<LevyAccountModel> paymentsLevyAccountDetails)
        {
            if (dasLevyAccountDetails.IsNullOrEmpty() || paymentsLevyAccountDetails.IsNullOrEmpty())
            {
                IsNullOrEmpty = true;
                return;
            }
            
            this.dasLevyAccountDetails = dasLevyAccountDetails.OrderBy(d => d.AccountId).ToList();
            this.paymentsLevyAccountDetails = paymentsLevyAccountDetails.OrderBy(p => p.AccountId).ToList();
            
            LevyAccounts = GetLevyAccounts();
        }

        public bool IsNullOrEmpty { get; }
        
        public int DasLevyAccountCount => dasLevyAccountDetails.Count;
        
        public int PaymentsLevyAccountCount => paymentsLevyAccountDetails.Count;

        public decimal DasLevyAccountBalanceTotal => dasLevyAccountDetails.Sum(d => d.Balance);

        public decimal PaymentsLevyAccountBalanceTotal => paymentsLevyAccountDetails.Sum(d => d.Balance);

        public decimal DasTransferAllowanceTotal => dasLevyAccountDetails.Sum(d => d.TransferAllowance);

        public decimal PaymentsTransferAllowanceTotal => paymentsLevyAccountDetails.Sum(d => d.TransferAllowance);

        public decimal DasIsLevyPayerCount => dasLevyAccountDetails.Count(d => d.IsLevyPayer);

        public decimal PaymentsIsLevyPayerCount => paymentsLevyAccountDetails.Count(d => d.IsLevyPayer);

        //This has to be a Property because Validator can't validate Methods
        public IEnumerable<LevyAccountsDto> LevyAccounts { get; }

        private IEnumerable<LevyAccountsDto> GetLevyAccounts()
        {
            var dasAccountsLookup = dasLevyAccountDetails.ToLookup(d => d.AccountId);
            var paymentsAccountLookup = paymentsLevyAccountDetails.ToLookup(p => p.AccountId);

            var dasAccountIds = dasAccountsLookup.Select(p => p.Key);
            var paymentsAccountIds = paymentsAccountLookup.Select(p => p.Key);

            var accountIds = new HashSet<long>(dasAccountIds.Union(paymentsAccountIds));

            var fullOuterJoin = from accountId in accountIds
                                from dasLevyAccount in dasAccountsLookup[accountId].DefaultIfEmpty(new LevyAccountModel())          //this makes it easier for Validator
                                from paymentsLevyAccount in paymentsAccountLookup[accountId].DefaultIfEmpty(new LevyAccountModel()) //no need to do null check
                                select new LevyAccountsDto { DasLevyAccount = dasLevyAccount, PaymentsLevyAccount = paymentsLevyAccount };

            return fullOuterJoin;
        }
    }
}
