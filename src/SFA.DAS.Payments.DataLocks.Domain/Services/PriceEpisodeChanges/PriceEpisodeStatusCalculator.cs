using System;
using System.Collections.Generic;
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

        public PriceEpisodeStatus DetermineStatus(short academicYear, PriceEpisode priceEpisode, List<OnProgrammeEarning> earnings, List<PriceEpisodeStatusChange> previousPriceEpisodeStatuses)
        {
            var previousPriceEpisode = previousPriceEpisodeStatuses.FirstOrDefault(previous =>
                previous.DataLock.PriceEpisodeIdentifier.Equals(priceEpisode.Identifier));
            if (previousPriceEpisode == null)
                return PriceEpisodeStatus.New;

            throw new NotImplementedException();
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
