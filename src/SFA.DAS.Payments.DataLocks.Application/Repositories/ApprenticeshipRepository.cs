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

            RemoveNonActivePriceEpisodes(apprenticeships);

            return apprenticeships;
        }

        public async Task<List<ApprenticeshipModel>> DuplicateApprenticeshipsForProvider(long ukprn)
        {
            var apprenticeshipDuplicateIds = await dataContext.ApprenticeshipDuplicate
                .Where(x => x.Ukprn == ukprn)
                .Select(x => x.ApprenticeshipId)
                .ToListAsync()
                .ConfigureAwait(false);

            var apprenticeships = await dataContext.Apprenticeship
                 .Include(x => x.ApprenticeshipPriceEpisodes)
                 .Where(x => apprenticeshipDuplicateIds.Contains(x.Id))
                 .ToListAsync()
                 .ConfigureAwait(false);

            RemoveNonActivePriceEpisodes(apprenticeships);

            return apprenticeships;

        }

        private static void RemoveNonActivePriceEpisodes(List<ApprenticeshipModel> apprenticeships)
        {
            apprenticeships.ForEach(x => x.ApprenticeshipPriceEpisodes = x.ApprenticeshipPriceEpisodes?
                .Where(o => !o.Removed)
                .ToList());
        }
    }
}
