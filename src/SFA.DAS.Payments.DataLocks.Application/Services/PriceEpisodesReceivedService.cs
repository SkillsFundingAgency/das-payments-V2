using Newtonsoft.Json;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public class PriceEpisodesReceivedService
    {
        private readonly ICurrentPriceEpisodeForJobStore currentPriceEpisodesStore;
        private readonly IReceivedDataLockEventStore receivedEventStore;

        public PriceEpisodesReceivedService(ICurrentPriceEpisodeForJobStore store, IReceivedDataLockEventStore receivedEventStore)
        {
            this.currentPriceEpisodesStore = store;
            this.receivedEventStore = receivedEventStore;
        }

        public DataLockStatusChanged JobSucceeded(long jobId, long ukprn)
        {
            // get received events from cache
            var received = receivedEventStore.GetDataLocks(jobId, ukprn);
            var deserialised = received.Select(x =>
            {
                var type = Type.GetType(typeof(PayableEarningEvent).AssemblyQualifiedName);
                var v = JsonConvert.DeserializeObject(x.Message, type);
                return (DataLockEvent)v;
            });

            // get current price episodes
            var currentPriceEpisodes = currentPriceEpisodesStore.GetCurentPriceEpisodes(jobId, ukprn);

            // calculate difference
            var matcher = new PriceEpisodeEventMatcher();
            var changes = matcher.Match(currentPriceEpisodes, deserialised.SelectMany(x => x.PriceEpisodes));

            // build events for approvals
            var buildEvents = new DataLockStatusChanged
            {
                PriceEpisodeStatusChanges = changes.Select(x => new PriceEpisodeStatusChange
                {
                    PriceEpisode = new PriceEpisode
                    {
                        Identifier = x.identifier,
                    },
                    Status = x.status,
                }).ToList(),
            };

            // update "current" with new events
            currentPriceEpisodesStore.Remove(jobId, ukprn);
            var a = deserialised.SelectMany(x => x.PriceEpisodes, (dlock, episode) =>
                    new CurrentPriceEpisode
                    {
                        JobId = jobId,
                        Ukprn = ukprn,
                        Uln = dlock.Learner.Uln,
                        PriceEpisodeIdentifier = episode.Identifier,
                        AgreedPrice = episode.AgreedPrice,
                    });
            currentPriceEpisodesStore.AddRange(a);


            // remove received events from cach
            receivedEventStore.Remove(jobId, ukprn);
        
            return buildEvents;
        }
    }
}
