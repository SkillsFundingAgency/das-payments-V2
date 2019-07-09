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
        Task<ApprenticeshipModel> UpdateApprenticeship(UpdatedApprenticeshipModel updatedApprenticeship);
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

        public async Task<ApprenticeshipModel> UpdateApprenticeship(UpdatedApprenticeshipModel updatedApprenticeship)
        {
            var currentApprenticeship = await repository.Get(updatedApprenticeship.ApprenticeshipId);
            if (currentApprenticeship == null)
            {
                throw new InvalidOperationException($"Cannot find updated apprenticeship Apprenticeship id: {updatedApprenticeship.ApprenticeshipId}");
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                currentApprenticeship.AgreedOnDate = updatedApprenticeship.AgreedOnDate;
                currentApprenticeship.Uln = updatedApprenticeship.Uln;
                currentApprenticeship.EstimatedStartDate = updatedApprenticeship.EstimatedStartDate;
                currentApprenticeship.EstimatedEndDate = updatedApprenticeship.EstimatedEndDate;
                currentApprenticeship.StandardCode = updatedApprenticeship.StandardCode;
                currentApprenticeship.ProgrammeType = updatedApprenticeship.ProgrammeType;
                currentApprenticeship.FrameworkCode = updatedApprenticeship.FrameworkCode;
                currentApprenticeship.PathwayCode = updatedApprenticeship.PathwayCode;

                UpdatePriceEpisodes(updatedApprenticeship.ApprenticeshipPriceEpisodes, currentApprenticeship.ApprenticeshipPriceEpisodes);

                var newPriceEpisodes = updatedApprenticeship.ApprenticeshipPriceEpisodes
                    .Where(x => currentApprenticeship.ApprenticeshipPriceEpisodes.All(o => o.StartDate != x.StartDate && o.Cost != x.Cost))
                    .ToList();

                currentApprenticeship.ApprenticeshipPriceEpisodes.AddRange(newPriceEpisodes);

                await repository.UpdateApprenticeship(currentApprenticeship);
                var latestApprenticeship = await repository.Get(updatedApprenticeship.ApprenticeshipId);

                scope.Complete();

                return latestApprenticeship;
            }
            
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
        }
    }


}
