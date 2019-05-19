using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ILevyAccountRepository
    {
        Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
