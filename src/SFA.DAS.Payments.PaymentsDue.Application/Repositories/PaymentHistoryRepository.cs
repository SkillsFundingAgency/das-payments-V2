using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public PaymentHistoryRepository(IRepositoryCache<IEnumerable<PaymentEntity>> cache, DedsContext dedsContext)
        {
            _cache = cache;
            _dedsContext = dedsContext;
        }

        public async Task<IEnumerable<Payment>> GetPaymentHistory(string apprenticeshipKey)
        {
            IEnumerable<PaymentEntity> entities;

            if (_cache != null && !_cache.IsInitialised)
                await InitialiseCache(apprenticeshipKey).ConfigureAwait(false);

            if (_cache == null)
                entities = GetEntities(apprenticeshipKey);
            else
                entities = await _cache.Get(apprenticeshipKey).ConfigureAwait(false);

            return entities.Select(Mapper.Map<PaymentEntity, Payment>).ToArray();
        }

        private async Task InitialiseCache(string apprenticeshipKey)
        {
            await _cache.Reset().ConfigureAwait(false);
            var entities = GetEntities(apprenticeshipKey);
            await _cache.Add(apprenticeshipKey, entities).ConfigureAwait(false);
            _cache.IsInitialised = true;
        }

        private PaymentEntity[] GetEntities(string apprenticeshipKey)
        {
            return _dedsContext.PaymentHistory.Where(p => p.ApprenticeshipKey == apprenticeshipKey).ToArray();
        }
    }
}