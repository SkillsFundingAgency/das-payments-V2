using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipService
    {
        Task<List<ApprenticeshipDuplicateModel>> NewApprenticeship(ApprenticeshipModel apprenticeship);
        Task<ApprenticeshipModel> UpdateApprenticeship(UpdatedApprenticeshipApprovedModel updatedApprenticeshipApproved);
        Task<ApprenticeshipModel> UpdateApprenticeshipForDataLockTriage(UpdatedApprenticeshipDataLockTriageModel updatedApprenticeship);
    }

    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly IApprenticeshipRepository repository;

        public ApprenticeshipService(IApprenticeshipRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ApprenticeshipDuplicateModel>> NewApprenticeship(ApprenticeshipModel newApprenticeship)
        {
            var apprenticeship = await repository.Get(newApprenticeship.Id);
            if (apprenticeship != null)
            {
                throw new InvalidOperationException($"Cannot store new apprenticeship as it already exists. Apprenticeship id: {newApprenticeship.Id}, employer: {newApprenticeship.AccountId}, ukprn: {newApprenticeship.Ukprn}");
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await repository.Add(newApprenticeship);
                var duplicates = await repository.GetDuplicates(newApprenticeship.Uln);
                var providers = duplicates
                    .Select(duplicate => duplicate.Ukprn)
                    .Distinct()
                    .ToList();

                var newDuplicates = providers
                    .Select(ukprn => new ApprenticeshipDuplicateModel
                    {
                        ApprenticeshipId = newApprenticeship.Id,
                        Ukprn = ukprn,
                        Uln = newApprenticeship.Uln
                    })
                    .ToList();

                if (!providers.Contains(newApprenticeship.Ukprn))
                {
                    newDuplicates.Add(new ApprenticeshipDuplicateModel { Ukprn = newApprenticeship.Ukprn, ApprenticeshipId = newApprenticeship.Id, Uln = newApprenticeship.Uln });
                    newDuplicates.AddRange(duplicates.Select(duplicate => new ApprenticeshipDuplicateModel
                    {
                        ApprenticeshipId = duplicate.ApprenticeshipId,
                        Uln = newApprenticeship.Uln,
                        Ukprn = newApprenticeship.Ukprn
                    }));
                }

                await repository.StoreDuplicates(newDuplicates);
                scope.Complete();
                return duplicates;
            }
        }

        public async Task<ApprenticeshipModel> UpdateApprenticeship(UpdatedApprenticeshipApprovedModel updatedApprenticeshipApproved)
        {
            var currentApprenticeship = await GetApprenticeship(updatedApprenticeshipApproved.ApprenticeshipId).ConfigureAwait(false);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                currentApprenticeship.AgreedOnDate = updatedApprenticeshipApproved.AgreedOnDate;
                currentApprenticeship.Uln = updatedApprenticeshipApproved.Uln;
                currentApprenticeship.EstimatedStartDate = updatedApprenticeshipApproved.EstimatedStartDate;
                currentApprenticeship.EstimatedEndDate = updatedApprenticeshipApproved.EstimatedEndDate;
                currentApprenticeship.StandardCode = updatedApprenticeshipApproved.StandardCode;
                currentApprenticeship.ProgrammeType = updatedApprenticeshipApproved.ProgrammeType;
                currentApprenticeship.FrameworkCode = updatedApprenticeshipApproved.FrameworkCode;
                currentApprenticeship.PathwayCode = updatedApprenticeshipApproved.PathwayCode;

                UpdatePriceEpisodes(updatedApprenticeshipApproved.ApprenticeshipPriceEpisodes, currentApprenticeship.ApprenticeshipPriceEpisodes);

                await repository.UpdateApprenticeship(currentApprenticeship);

                var latestApprenticeship = await GetApprenticeship(updatedApprenticeshipApproved.ApprenticeshipId).ConfigureAwait(false);

                scope.Complete();

                return latestApprenticeship;
            }
        }

        public async Task<ApprenticeshipModel> UpdateApprenticeshipForDataLockTriage(UpdatedApprenticeshipDataLockTriageModel updatedApprenticeship)
        {
            var currentApprenticeship = await GetApprenticeship(updatedApprenticeship.ApprenticeshipId).ConfigureAwait(false);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                currentApprenticeship.AgreedOnDate = updatedApprenticeship.AgreedOnDate;
                currentApprenticeship.StandardCode = updatedApprenticeship.StandardCode;
                currentApprenticeship.ProgrammeType = updatedApprenticeship.ProgrammeType;
                currentApprenticeship.FrameworkCode = updatedApprenticeship.FrameworkCode;
                currentApprenticeship.PathwayCode = updatedApprenticeship.PathwayCode;

                UpdatePriceEpisodes(updatedApprenticeship.ApprenticeshipPriceEpisodes, currentApprenticeship.ApprenticeshipPriceEpisodes);
                
                await repository.UpdateApprenticeship(currentApprenticeship);
                var latestApprenticeship = await GetApprenticeship(updatedApprenticeship.ApprenticeshipId).ConfigureAwait(false);

                scope.Complete();

                return latestApprenticeship;
            }
        }

        private async Task<ApprenticeshipModel> GetApprenticeship(long apprenticeshipId)
        {
            var currentApprenticeship = await repository.Get(apprenticeshipId);
            if (currentApprenticeship == null)
            {
                throw new InvalidOperationException(
                    $"Cannot find updated apprenticeship Apprenticeship id: {apprenticeshipId}");
            }

            return currentApprenticeship;
        }
        
        private void UpdatePriceEpisodes(List<ApprenticeshipPriceEpisodeModel> receivedPriceEpisodes, List<ApprenticeshipPriceEpisodeModel> currentPriceEpisodes)
        {
            foreach (var currentPriceEpisode in currentPriceEpisodes)
            {
                var updatedPriceEpisode = receivedPriceEpisodes
                    .SingleOrDefault(o => o.StartDate == currentPriceEpisode.StartDate && o.Cost == currentPriceEpisode.Cost);

                if (updatedPriceEpisode == null)
                {
                    currentPriceEpisode.Removed = true;
                }
                else
                {
                    currentPriceEpisode.EndDate = updatedPriceEpisode.EndDate;
                }
            }

            var newPriceEpisodes = receivedPriceEpisodes
                .Where(x => currentPriceEpisodes.All(o => o.StartDate != x.StartDate && o.Cost != x.Cost))
                .ToList();

            currentPriceEpisodes.AddRange(newPriceEpisodes);
        }
    }
}
