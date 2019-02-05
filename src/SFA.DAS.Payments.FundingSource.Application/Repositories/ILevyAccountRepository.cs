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

        public LevyAccountRepository(PaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.LevyAccount
                .Where(levyAccount => levyAccount.AccountId == employerAccountId)
                .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
