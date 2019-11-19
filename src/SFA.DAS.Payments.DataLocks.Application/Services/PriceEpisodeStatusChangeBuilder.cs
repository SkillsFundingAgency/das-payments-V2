using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class PriceEpisodeStatusChangeBuilder
    {
        public List<PriceEpisodeStatusChange> Build(List<DataLockEvent> dataLockEvents, List<(string identifier, PriceEpisodeStatus status)> priceEpisodeChanges)
        {
            var priceEpisodesWithDatalocks = dataLockEvents
                .SelectMany(
                    dl => dl.PriceEpisodes, 
                    (datalock, priceEpisode) => (datalock, priceEpisode));

            var changes = priceEpisodeChanges
                .LeftOuterJoin(priceEpisodesWithDatalocks, 
                    left => left.identifier,
                    right => right.priceEpisode.Identifier,
                    (left, right) => 
                        (left.identifier, right.priceEpisode, right.datalock, left.status))
                .ToList();

            return changes.Select(MapPriceEpisodeStatusChange).ToList();

            //foreach (var priceEpisodeChange in priceEpisodeChanges)
            //{
            //    var priseEpisode = dataLockEvents.SelectMany(x => x.PriceEpisodes).FirstOrDefault(x => x.Identifier == priceEpisodeChange.identifier);

            //    var priseEpisodeStatusChange = new PriceEpisodeStatusChange
            //    {
            //        CollectionPeriod = DataLockEvent.
            //    }

            //}
        }

        private static PriceEpisodeStatusChange MapPriceEpisodeStatusChange(
            DataLockEvent datalock, 
            string priceEpisodeIdentifier, 
            PriceEpisodeStatus status, 
            PriceEpisode priceEpisode)
        {
            return new PriceEpisodeStatusChange
            {
                JobId = datalock?.JobId ?? 0,
                Ukprn = datalock?.Ukprn ?? 0,
                DataLock = new LegacyDataLockEvent
                {
                    PriceEpisodeIdentifier = priceEpisodeIdentifier,
                    Status = status,
                    HasErrors = datalock is EarningFailedDataLockMatching,
                    // ...
                },
            };
        }

        private static PriceEpisodeStatusChange MapPriceEpisodeStatusChange(
            (string priceEpisodeIdentifier,
             PriceEpisode priceEpisode,
             DataLockEvent datalock,
             PriceEpisodeStatus status) change)
            => MapPriceEpisodeStatusChange(
                change.datalock,
                change.priceEpisodeIdentifier,
                change.status,
                change.priceEpisode);
    }
}
