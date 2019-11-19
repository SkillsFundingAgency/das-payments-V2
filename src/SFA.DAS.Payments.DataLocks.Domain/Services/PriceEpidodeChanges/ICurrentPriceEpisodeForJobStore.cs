using SFA.DAS.Payments.DataLocks.Messages.Events;
using System.Collections.Generic;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges
{
    public interface ICurrentPriceEpisodeForJobStore
    {
        void Add(CurrentPriceEpisode priceEpisode);
        void AddRange(IEnumerable<CurrentPriceEpisode> priceEpisode);
        IEnumerable<CurrentPriceEpisode> GetCurentPriceEpisodes(long jobId, long ukprn);
        void Remove(long jobId, long ukprn);
    }


    public class ReceivedDataLockEvent
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public long Ukprn { get; set; }
        public string MessageType { get; set; }
        public string Message { get; set; }
    }

    public interface IReceivedDataLockEventStore
    {
        void Add(ReceivedDataLockEvent dataLock);
        IEnumerable<ReceivedDataLockEvent> GetDataLocks(long jobId, long ukprn);
        //IEnumerable<CurrentPriceEpisode> GetCurentPriceEpisodes(long jobId, long ukprn);
        void Remove(long jobId, long ukprn);
    }
}
