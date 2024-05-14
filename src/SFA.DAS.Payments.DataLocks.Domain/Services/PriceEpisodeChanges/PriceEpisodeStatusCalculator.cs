using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpisodeChanges
{

    public class PriceEpisodeStatusCalculator
    {
        public List<(string identifier, PriceEpisodeStatus status)> Calculate(
            IEnumerable<CurrentPriceEpisode> currentPriceEpisodes,
            IEnumerable<PriceEpisode> receivedPriceEpisodes)
        {
            var matched = receivedPriceEpisodes
                .Select(x => (x.Identifier, Match(currentPriceEpisodes, x)));

            var receivedIdentifiers =
                receivedPriceEpisodes.Select(x => x.Identifier);

            var removed = currentPriceEpisodes
                .Select(x => x.PriceEpisodeIdentifier)
                .Except(receivedIdentifiers)
                .Select(id => (id, PriceEpisodeStatus.Removed));

            return matched.Union(removed).ToList();
        }


        
        public PriceEpisodeStatus DetermineDataLockStatus(short academicYear, PriceEpisode priceEpisode, List<OnProgrammeEarning> earnings, List<PriceEpisodeStatusChange> previousPriceEpisodeStatuses)
        {
            //make sure only checking price episodes that are in scope
            var earningPeriodsForPriceEpisode = earnings.SelectMany(earning => earning.Periods)
                .Where(period => !string.IsNullOrEmpty(period.PriceEpisodeIdentifier) && period.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier))
                .ToList();

            var filteredPreviousStatuses = previousPriceEpisodeStatuses.Where(previous =>
                previous.DataLock.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier)
                && previous.DataLock.AcademicYear.Equals(academicYear.ToString()))
                .ToList();
            
            //Price episode identifier contains the academic year but adding extra check for safety e.g. 25-237-11/01/2024
            var previousPriceEpisode = filteredPreviousStatuses.FirstOrDefault();
            if (previousPriceEpisode == null)
                return PriceEpisodeStatus.New;

            if (previousPriceEpisode.AgreedPrice != priceEpisode.AgreedPrice)
                return PriceEpisodeStatus.Updated;

            var previousDataLocks = filteredPreviousStatuses
                .SelectMany(previous => previous.Errors)
                .Select(er => er.ErrorCode)
                .Distinct()
                .ToList();
            var currentDataLocks = earningPeriodsForPriceEpisode
                .SelectMany(period => period.DataLockFailures)
                .Select(failure => failure.DataLockError.ToString())
                .Distinct()
                .ToList();

            if (currentDataLocks.Except(previousDataLocks).Any())
                return PriceEpisodeStatus.Updated;

            var previousCommitments =
                filteredPreviousStatuses.Select(previous => previous.DataLock.CommitmentId)
                    .Distinct()
                    .ToList();
            var currentApprenticeships = earningPeriodsForPriceEpisode
                .Where(period => period.ApprenticeshipId.HasValue)
                .Select(period => period.ApprenticeshipId.Value)
                .Distinct()
                .ToList();

            if (currentApprenticeships.Except(previousCommitments).Any())
                return PriceEpisodeStatus.Updated;


            var commitmentErrors = filteredPreviousStatuses
                .GroupBy(x => x.DataLock.CommitmentId, x => new {Id = x.DataLock.CommitmentId, Errors = x.Errors} )
                .ToList();

            //Edge case: Same learners
            foreach (var commitmentErrorGroup in commitmentErrors)
            {
                var errors = commitmentErrorGroup.SelectMany(x => x.Errors).Select(x => x.ErrorCode).Distinct().ToList();
                if (earningPeriodsForPriceEpisode
                    .Where(period => period.ApprenticeshipId == commitmentErrorGroup.Key)
                    .SelectMany(period => period.DataLockFailures)
                    .Select(failure => failure.DataLockError.ToString())
                    .Distinct()
                    .Except(errors).Any())
                    return PriceEpisodeStatus.Updated;
            }

            return PriceEpisodeStatus.NoCHange;
        }

        
        public PriceEpisodeStatus Match(
            IEnumerable<CurrentPriceEpisode> recordedPriceEpisodes,
            PriceEpisode newPriceEpisode)
        {
            var previous = recordedPriceEpisodes.FirstOrDefault(MatchById(newPriceEpisode));

            if (previous == null)
                return PriceEpisodeStatus.New;

            if (previous.AgreedPrice == newPriceEpisode.AgreedPrice)
                return PriceEpisodeStatus.New;

            return PriceEpisodeStatus.Updated;
        }

        private Func<CurrentPriceEpisode, bool> MatchById(PriceEpisode find) =>
            x => x.PriceEpisodeIdentifier.Equals(find.Identifier, StringComparison.InvariantCultureIgnoreCase);
    }
}
