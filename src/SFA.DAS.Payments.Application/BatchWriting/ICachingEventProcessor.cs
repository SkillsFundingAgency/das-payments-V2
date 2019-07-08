using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace SFA.DAS.Payments.Application.BatchWriting
{
    public interface ICachingEventProcessor<T>
    {
        Task EnqueueEvent(T message, CancellationToken cancellationToken);
    }

    public class CachingEventProcessor<T> : ICachingEventProcessor<T>
    {
        private readonly IMapper mapper;
        private readonly IBatchedDataCache<T> cache;

        public CachingEventProcessor(IMapper mapper, IBatchedDataCache<T> cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task EnqueueEvent(T message, CancellationToken cancellationToken)
        {
            var model = mapper.Map<T>(message);  
            await cache.AddPayment(model, cancellationToken).ConfigureAwait(false);
        }
    }
}
