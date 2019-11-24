using Newtonsoft.Json;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpisodeChanges;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{

    public interface IPriceEpisodesReceivedService
    {
        Task<List<PriceEpisodeStatusChange>> JobSucceeded(long jobId, long ukprn);
    }


    public class PriceEpisodesReceivedService: IPriceEpisodesReceivedService
    {
        private readonly ICurrentPriceEpisodeForJobStore currentPriceEpisodesStore;
        private readonly IReceivedDataLockEventStore receivedEventStore;
        private readonly PriceEpisodeStatusChangeBuilder statusChangeBuilder;

        public PriceEpisodesReceivedService(ICurrentPriceEpisodeForJobStore store,
            IReceivedDataLockEventStore receivedEventStore,
            PriceEpisodeStatusChangeBuilder statusChangeBuilder)
        {
            currentPriceEpisodesStore = store;
            this.receivedEventStore = receivedEventStore;
            this.statusChangeBuilder = statusChangeBuilder;
        }

        public async Task<List<PriceEpisodeStatusChange>> JobSucceeded(long jobId, long ukprn)
        {
            var priceEpisodeReplacements = new List<CurrentPriceEpisode>();
            var allPriceEpisodeStatusChanges = new List<PriceEpisodeStatusChange>();

            var dataLockEvents = (await GetDataLocks(jobId, ukprn)).ToList();
            var learnerUlns = dataLockEvents.Select(d => d.Learner.Uln).Distinct();

            foreach (var learnerUln in learnerUlns)
            {
                var learnerDataLocks = dataLockEvents.Where(x => x.Learner.Uln == learnerUln).ToList();

                var currentPriceEpisodes = await GetCurrentPriceEpisodes(jobId, ukprn, learnerUln);
                var changes = CalculatePriceEpisodeStatus(learnerDataLocks, currentPriceEpisodes);
                var priceEpisodeStatusChanges = await CreateStatusChangedEvents(learnerDataLocks, changes);
                var learnerPriceEpisodeReplacements = CreateLearnerCurrentPriceEpisodesReplacement(jobId, ukprn, learnerUln, priceEpisodeStatusChanges);
                priceEpisodeReplacements.AddRange(learnerPriceEpisodeReplacements);

                allPriceEpisodeStatusChanges.AddRange(priceEpisodeStatusChanges);
            }

            await ReplaceCurrentPriceEpisodes(jobId, ukprn, priceEpisodeReplacements);
            await RemoveReceivedDataLockEvents(jobId, ukprn);

            return allPriceEpisodeStatusChanges;
        }

        private async Task<IEnumerable<DataLockEvent>> GetDataLocks(long jobId, long ukprn)
        {
            var receivedEvents = await receivedEventStore.GetDataLocks(jobId, ukprn);
            var dataLocks = receivedEvents.Select(x =>
            {
                var type = Type.GetType(x.MessageType);
                return (DataLockEvent)JsonConvert.DeserializeObject(x.Message, type);
            });
            return dataLocks;
        }

        private Task<IEnumerable<CurrentPriceEpisode>> GetCurrentPriceEpisodes(long jobId, long ukprn, long uln)
        {
            return currentPriceEpisodesStore.GetCurrentPriceEpisodes(jobId, ukprn, uln);
        }

        private static List<(string identifier, PriceEpisodeStatus status)> CalculatePriceEpisodeStatus(
            IEnumerable<DataLockEvent> dataLocks, IEnumerable<CurrentPriceEpisode> currentPriceEpisodes)
        {
            var calculator = new PriceEpisodeStatusCalculator();
            var changes = calculator.Calculate(currentPriceEpisodes, dataLocks.SelectMany(x => x.PriceEpisodes));
            return changes;
        }

        private async Task<List<PriceEpisodeStatusChange>> CreateStatusChangedEvents(
            IEnumerable<DataLockEvent> dataLocks, List<(string identifier, PriceEpisodeStatus status)> changes)
        {
            return  await statusChangeBuilder.Build(dataLocks.ToList(), changes);
        }


        private  List<CurrentPriceEpisode> CreateLearnerCurrentPriceEpisodesReplacement(
            long jobId,
            long ukprn,
            long uln,
            IEnumerable<PriceEpisodeStatusChange> episodeStatusChanges)
        {
            var replacements = episodeStatusChanges
                .GroupBy(o => new
                {
                    JobId = jobId,
                    Ukprn = ukprn,
                    Uln = uln,
                    PriceEpisodeIdentifier = o.DataLock.PriceEpisodeIdentifier,
                    AgreedPrice = o.AgreedPrice
                })
                .Select(x =>
                {
                    var messages = x.Select(o => o).ToList();
                    return new CurrentPriceEpisode
                        {
                            JobId = x.Key.JobId,
                            Ukprn = x.Key.Ukprn,
                            Uln = x.Key.Uln,
                            PriceEpisodeIdentifier = x.Key.PriceEpisodeIdentifier,
                            AgreedPrice = x.Key.AgreedPrice,
                            MessageType =messages.GetType().AssemblyQualifiedName,
                            Message = JsonConvert.SerializeObject(messages)
                        };
                    }).ToList();

            return replacements;
        }

        private async Task ReplaceCurrentPriceEpisodes(
            long jobId,
            long ukprn,
            IEnumerable<CurrentPriceEpisode> currentPriceEpisodes)
        {
            
            await currentPriceEpisodesStore.Replace(jobId, ukprn, currentPriceEpisodes);
        }

        private Task RemoveReceivedDataLockEvents(long jobId, long ukprn)
        {
            return receivedEventStore.Remove(jobId, ukprn);
        }
    }
}
