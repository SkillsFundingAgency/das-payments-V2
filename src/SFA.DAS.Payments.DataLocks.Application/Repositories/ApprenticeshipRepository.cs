using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task<List<ApprenticeshipModel>> ApprenticeshipsForProvider(long ukprn);
    }

    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public ApprenticeshipRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<ApprenticeshipModel>> ApprenticeshipsForProvider(long ukprn)
        {
            var apprenticeships = await dataContext.Apprenticeship
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync()
                .ConfigureAwait(false);

            return apprenticeships;
        }
    }
}
