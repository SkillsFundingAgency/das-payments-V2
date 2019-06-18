using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ILevyFundingSourceRepository
    {
        Task<LevyAccountModel> GetLevyAccount(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<EmployerProviderPriorityModel>> GetPaymentPriorities(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
