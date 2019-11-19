using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges
{
    public class CurrentPriceEpisode
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AgreedPrice { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
    }

    public class PriceEpisodeEventMatcher
    {
        public List<(string identifier, PriceEpisodeStatus status)> Match(
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
