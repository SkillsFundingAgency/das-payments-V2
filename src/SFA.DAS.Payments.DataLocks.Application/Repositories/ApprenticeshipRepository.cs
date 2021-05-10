﻿using System;
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

        public async Task<List<long>> GetProviderIdsByUln(long uln)
        {
            return await dataContext.Apprenticeship
                .Where(x => x.Ukprn != 0 && x.Uln == uln)
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
                .Include(x => x.ApprenticeshipPauses)
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
        
        //public async Task<EarningEventModel> GetLatestProviderApprenticeshipEarnings(long uln, long ukprn, String eventType, CancellationToken cancellationToken)
        //{
        //    var apprenticeshipEarning = await dataContext.EarningEvent
        //        .Include(x => x.Periods)
        //        .Include(x => x.PriceEpisodes)
        //        .Where(x => x.Ukprn == ukprn && 
        //                    x.LearnerUln == uln && 
        //                    x.ContractType == ContractType.Act1 &&
        //                    x.EventType == eventType)
        //        .OrderByDescending(x => x.Id)
        //        .Take(1)
        //        .FirstOrDefaultAsync(cancellationToken)
        //        .ConfigureAwait(false);

        //    return apprenticeshipEarning;
        //}
        
        public async Task<List<ApprenticeshipModel>> GetEmployerApprenticeships(long accountId, CancellationToken cancellationToken)
        {
            var employerApprenticeships = await dataContext.Apprenticeship
                .Where(x => x.AccountId == accountId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return employerApprenticeships;
        }

        public async Task UpdateApprenticeships(List<ApprenticeshipModel> updatedApprenticeships)
        {
            foreach (var updatedApprenticeship in updatedApprenticeships)
            {
               dataContext.Apprenticeship.Update(updatedApprenticeship);
            }
            
            await dataContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private static void RemoveNonActivePriceEpisodes(List<ApprenticeshipModel> apprenticeships)
        {
            apprenticeships.ForEach(x => x.ApprenticeshipPriceEpisodes = x.ApprenticeshipPriceEpisodes?
                .Where(o => !o.Removed)
                .ToList());
        }



        public void Dispose()
        {
            (dataContext as PaymentsDataContext)?.Dispose();
        }
    }
}
