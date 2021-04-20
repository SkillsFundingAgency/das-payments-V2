﻿using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public abstract class ApprenticeshipUpdatedService<T> : IApprenticeshipUpdatedService<T> where T : BaseUpdatedApprenticeshipModel
    {
        protected readonly IApprenticeshipRepository repository;

        protected ApprenticeshipUpdatedService(IApprenticeshipRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ApprenticeshipModel> UpdateApprenticeship(T updatedApprenticeship)
        {
            var currentApprenticeship = await GetApprenticeship(updatedApprenticeship.ApprenticeshipId).ConfigureAwait(false);

            HandleUpdated(currentApprenticeship, updatedApprenticeship);

            if (updatedApprenticeship.ApprenticeshipPriceEpisodes != null && updatedApprenticeship.ApprenticeshipPriceEpisodes.Any())
            {
                UpdatePriceEpisodes(updatedApprenticeship.ApprenticeshipPriceEpisodes, currentApprenticeship.ApprenticeshipPriceEpisodes);
            }

            UpdateHistory(currentApprenticeship, updatedApprenticeship);

            await repository.UpdateApprenticeship(currentApprenticeship);

            var latestApprenticeship = await GetApprenticeship(updatedApprenticeship.ApprenticeshipId).ConfigureAwait(false);

            return latestApprenticeship;
        }

        protected abstract void HandleUpdated(ApprenticeshipModel current, T updated);

        protected virtual void UpdateHistory(ApprenticeshipModel current, T updated)
        {
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
                .Where(x => currentPriceEpisodes.All(o => o.Cost != x.Cost || o.StartDate != x.StartDate))
                .ToList();

            currentPriceEpisodes.AddRange(newPriceEpisodes);
        }
    }
}