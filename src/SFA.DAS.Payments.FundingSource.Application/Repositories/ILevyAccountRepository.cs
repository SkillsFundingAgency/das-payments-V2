using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public interface ILevyAccountRepository
    {
        Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class LevyAccountRepository : ILevyAccountRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public LevyAccountRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.LevyAccount.AsNoTracking()
                .Where(levyAccount => levyAccount.AccountId == employerAccountId)
                .SingleAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
