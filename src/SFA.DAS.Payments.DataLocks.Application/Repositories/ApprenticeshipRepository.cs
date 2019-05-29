using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task<List<ApprenticeshipModel>> ApprenticeshipsForProvider(long ukprn);
        Task<List<ApprenticeshipModel>> DuplicateApprenticeshipsForProvider(long ukprn);
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
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync()
                .ConfigureAwait(false);

            apprenticeships?.ForEach( x=> x.ApprenticeshipPriceEpisodes = x.ApprenticeshipPriceEpisodes?
                                                                            .Where(o => !o.Removed)
                                                                            .ToList());

            return apprenticeships;
        }

        public async Task<List<ApprenticeshipModel>> DuplicateApprenticeshipsForProvider(long ukprn)
        {
            var apprenticeshipDuplicates = await dataContext.ApprenticeshipDuplicate
                .Include(x => x.Apprenticeship)
                .Where(x => x.Ukprn == ukprn)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var apprenticeshipDuplicate in apprenticeshipDuplicates)
            {
                apprenticeshipDuplicate.Apprenticeship.ApprenticeshipPriceEpisodes = await dataContext.ApprenticeshipPriceEpisode
                    .Where(x => !x.Removed && x.ApprenticeshipId == apprenticeshipDuplicate.Apprenticeship.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);
            }

            return apprenticeshipDuplicates.Select(x => x.Apprenticeship).ToList();
        }

    }
}
