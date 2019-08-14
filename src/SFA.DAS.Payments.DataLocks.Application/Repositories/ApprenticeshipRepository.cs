using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public class ApprenticeshipRepository : IApprenticeshipRepository
    {
        private readonly IPaymentsDataContext dataContext;

        public ApprenticeshipRepository(IPaymentsDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<List<long>> GetProviderIds()
        {
            return await dataContext.Apprenticeship
                .Where(x => x.Ukprn != 0)
                .Select(x => x.Ukprn)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<long>> ApprenticeshipUlnsByProvider(long ukprn)
        {
            var apprenticeships = await dataContext.Apprenticeship
                .Where(x => x.Ukprn == ukprn)
                .Select(x => x.Uln)
                .ToListAsync()
                .ConfigureAwait(false);
            return apprenticeships;
        }

        public async Task<List<ApprenticeshipModel>> ApprenticeshipsByUln(long uln)
        {
            var apprenticeships = await dataContext.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(x => x.Uln == uln)
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

        public async Task<ApprenticeshipModel> Get(long apprenticeshipId)
        {
            return await dataContext.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .FirstOrDefaultAsync(apprenticeship => apprenticeship.Id == apprenticeshipId)
                .ConfigureAwait(false);
        }

        public async Task<List<ApprenticeshipModel>> Get(List<long> apprenticeshipIds, CancellationToken cancellationToken)
        {
            return await dataContext.Apprenticeship
                .Include(x => x.ApprenticeshipPriceEpisodes)
                .Where(apprenticeship => apprenticeshipIds.Contains(apprenticeship.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<List<ApprenticeshipDuplicateModel>> GetDuplicates(long uln)
        {
            return await dataContext.ApprenticeshipDuplicate
                .Where(duplicate => duplicate.Uln == uln)
                .ToListAsync();
        }

        public async Task Add(ApprenticeshipModel apprenticeship)
        {
            dataContext.Apprenticeship.Add(apprenticeship);
            await dataContext.SaveChangesAsync();
        }

        public async Task StoreDuplicates(List<ApprenticeshipDuplicateModel> duplicates)
        {
            dataContext.ApprenticeshipDuplicate.AddRange(duplicates);
            await dataContext.SaveChangesAsync();
        }

        private static void RemoveNonActivePriceEpisodes(List<ApprenticeshipModel> apprenticeships)
        {
            apprenticeships.ForEach(x => x.ApprenticeshipPriceEpisodes = x.ApprenticeshipPriceEpisodes?
                .Where(o => !o.Removed)
                .ToList());
        }

        public async Task UpdateApprenticeship(ApprenticeshipModel updatedApprenticeship)
        {
            dataContext.Apprenticeship.Update(updatedApprenticeship);
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task AddApprenticeshipPause(ApprenticeshipPauseModel pauseModel)
        {
            dataContext.ApprenticeshipPause.Add(pauseModel);
            await dataContext.SaveChangesAsync();
        }

        public  async Task<ApprenticeshipPauseModel> GetCurrentApprenticeshipPausedModel(long apprenticeshipId)
        {
            var currentlyPausedApprenticeship = await dataContext.ApprenticeshipPause
                .Where(x => x.ApprenticeshipId == apprenticeshipId)
                .OrderByDescending(x => x.PauseDate)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return currentlyPausedApprenticeship;
        }

        public async Task UpdateCurrentlyPausedApprenticeship(ApprenticeshipPauseModel apprenticeshipPauseModel)
        {
            dataContext.ApprenticeshipPause.Update(apprenticeshipPauseModel);
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }


        public async Task<List<EarningEventModel>> GetProviderApprenticeshipEarnings(long uln, long ukprn)
        {

            var earningDbModel = new EarningEventModel();

            var earningModel = new ApprenticeshipContractType1EarningEvent
            {
                Ukprn = earningDbModel.Ukprn,
                AgreementId = earningDbModel.AgreementId,
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(earningDbModel.AcademicYear, earningDbModel.CollectionPeriod),
                CollectionYear = earningDbModel.AcademicYear,
                EventId = earningDbModel.EventId,
                EventTime = earningDbModel.EventTime,
                // IlrFileName =  earningDbModel.
                IlrSubmissionDateTime = DateTime.UtcNow,
                JobId = earningDbModel.JobId,
                // SfaContributionPercentage = earningDbModel.s
                PriceEpisodes = earningDbModel.PriceEpisodes.Select(x => new PriceEpisode
                {
                    Identifier = x.PriceEpisodeIdentifier,
                   // AgreedPrice = 
                    TotalNegotiatedPrice1 = x.TotalNegotiatedPrice1
                }).ToList()
            };




        }


        public void Dispose()
        {
            (dataContext as PaymentsDataContext)?.Dispose();
        }
    }
}
