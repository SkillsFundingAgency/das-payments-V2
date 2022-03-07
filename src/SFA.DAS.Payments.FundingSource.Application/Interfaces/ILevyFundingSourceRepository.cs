using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ILevyFundingSourceRepository
    {
        Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<Tuple<long, long?>>> GetEmployerAccountsByUkprn(long ukprn, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<long>> GetEmployerAccounts(CancellationToken cancellationToken);
        Task<List<(long AccountId, bool IsLevyPayer)>> GetCurrentEmployerStatus(List<long> employerIds, CancellationToken cancellationToken);
        Task SaveLevyAccountAuditModel(long accountId, short academicYear, byte collectionPeriod, decimal sourceLevyAccountBalance, decimal adjustedLevyAccountBalance, decimal sourceTransferAllowance, decimal adjustedTransferAllowance, bool isLevyPayer);
    }
}
