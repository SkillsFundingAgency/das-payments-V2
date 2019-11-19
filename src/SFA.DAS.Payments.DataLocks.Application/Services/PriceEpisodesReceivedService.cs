using Newtonsoft.Json;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<DataLockStatusChanged> JobSucceeded(long jobId, long ukprn)
        {
            var datalocks = GetDatalocks(jobId, ukprn);

            var currentPriceEpisodes = await GetCurrentPriceEpisodes(jobId, ukprn);

            var changes = CalculatePriceEpisodeStatus(datalocks, currentPriceEpisodes);

            var buildEvents = CreateStatusChangedEvents(changes);

            await ReplaceCurrentPriceEpisodes(jobId, ukprn, datalocks);

            RemoveReceivedDataLockEvents(jobId, ukprn);

            return buildEvents;
        }

        private IEnumerable<DataLockEvent> GetDatalocks(long jobId, long ukprn)
        {
            var receivedEvents = receivedEventStore.GetDataLocks(jobId, ukprn);
            var datalocks = receivedEvents.Select(x =>
            {
                var type = Type.GetType(typeof(PayableEarningEvent).AssemblyQualifiedName);
                return (DataLockEvent)JsonConvert.DeserializeObject(x.Message, type);
            });
            return datalocks;
        }

        private Task<IEnumerable<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long jobId, long ukprn)
        {
            return currentPriceEpisodesStore.GetCurentPriceEpisodes(jobId, ukprn);
        }

        private static List<(string identifier, PriceEpisodeStatus status)> CalculatePriceEpisodeStatus(
            IEnumerable<DataLockEvent> datalocks, IEnumerable<CurrentPriceEpisode> currentPriceEpisodes)
        {
            var calculator = new PriceEpisodeStatusCalculator();
            var changes = calculator.Calculate(currentPriceEpisodes, datalocks.SelectMany(x => x.PriceEpisodes));
            return changes;
        }

        private static DataLockStatusChanged CreateStatusChangedEvents(
            List<(string identifier, PriceEpisodeStatus status)> changes)
        {
            return new DataLockStatusChanged
            {
                PriceEpisodeStatusChanges = changes.Select(MapToPriceEpisodeStatusChange).ToList(),
            };
        }

        private static PriceEpisodeStatusChange MapToPriceEpisodeStatusChange(
            (string identifier, PriceEpisodeStatus status) priceEpisode)
            => MapToPriceEpisodeStatusChange(priceEpisode.identifier, priceEpisode.status);

        private static PriceEpisodeStatusChange MapToPriceEpisodeStatusChange(
            string identifier, PriceEpisodeStatus status)
        {
            return new PriceEpisodeStatusChange
            {
                PriceEpisode = new PriceEpisode { Identifier = identifier },
                Status = status,
            };
        }

        private async Task ReplaceCurrentPriceEpisodes(
            long jobId, long ukprn, IEnumerable<DataLockEvent> datalocks)
        {
            var replacement = datalocks
                .SelectMany(x => x.PriceEpisodes, (dlock, episode) =>
                    new CurrentPriceEpisode
                    {
                        JobId = jobId,
                        Ukprn = ukprn,
                        Uln = dlock.Learner.Uln,
                        PriceEpisodeIdentifier = episode.Identifier,
                        AgreedPrice = episode.AgreedPrice,
                    });

            await currentPriceEpisodesStore.Replace(jobId, ukprn, replacement);
        }

        private void RemoveReceivedDataLockEvents(long jobId, long ukprn)
        {
            receivedEventStore.Remove(jobId, ukprn);
        }
    }
}
