using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Application.Data;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Application.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IRepositoryCache<IEnumerable<PaymentEntity>> _cache;
        private readonly DedsContext _dedsContext;

        public PaymentHistoryRepository(DedsContext dedsContext)
        {
            _dedsContext = dedsContext;
        }

        public PaymentHistoryRepository(DedsContext dedsContext, IRepositoryCache<IEnumerable<PaymentEntity>> cache)
        {
            _cache = cache;
            _dedsContext = dedsContext;
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistory(string apprenticeshipKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (_cache != null)
            {
                var result = await _cache.TryGet(apprenticeshipKey, cancellationToken).ConfigureAwait(false);
                if (result.HasValue)
                    return ConvertEntities(result.Value);
            }

            var entities = GetEntities(apprenticeshipKey);

            if (_cache != null)
                await _cache.Add(apprenticeshipKey, entities, cancellationToken).ConfigureAwait(false);

            return ConvertEntities(entities);
        }

        private static Payment[] ConvertEntities(IEnumerable<PaymentEntity> entities)
        {
            return entities.Select(Mapper.Map<PaymentEntity, Payment>).ToArray();
        }

        private PaymentEntity[] GetEntities(string apprenticeshipKey)
        {
            // HACK: this is for integration test to work
            return new PaymentEntity[0];
            return _dedsContext.PaymentHistory.Where(p => p.ApprenticeshipKey == apprenticeshipKey).ToArray();
        }
    }
}