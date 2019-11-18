using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class CurrentPriceEpisode
    {
        public string PriceEpisodeIdentifier { get; set; }
        public decimal AgreedPrice { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
    }

    public enum PriceEpisodeStatus
    {
        New = 1,
        Updated,
        Removed,
    }

    public class PriceEpisodeEventMatcher
    {
        public PriceEpisodeStatus Match(IEnumerable<CurrentPriceEpisode> recordedPriceEpisodes, PriceEpisode newPriceEpisode)
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
