using SFA.DAS.Payments.DataLocks.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.PriceEpidodeChanges
{
    public interface ICurrentPriceEpisodeForJobStore
    {
        Task Add(CurrentPriceEpisode priceEpisode);
        Task<IEnumerable<CurrentPriceEpisode>> GetCurentPriceEpisodes(long jobId, long ukprn);
        Task Replace(long jobId, long ukprn, IEnumerable<CurrentPriceEpisode> replacement);
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
