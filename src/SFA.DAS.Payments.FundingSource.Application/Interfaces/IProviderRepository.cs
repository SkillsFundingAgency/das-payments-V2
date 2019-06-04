using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IProviderRepository
    {
        Task<List<long>> GetApprenticeshipEmployers(long ukprn);
    }
}