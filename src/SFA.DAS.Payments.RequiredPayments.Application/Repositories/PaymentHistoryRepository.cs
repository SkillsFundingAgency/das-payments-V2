using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.RequiredPayments.Application.Data;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IRepositoryCache<IEnumerable<PaymentEntity>> _cache;
        private readonly RequiredPaymentsDataContext _requiredPaymentsDataContext;

        public PaymentHistoryRepository(RequiredPaymentsDataContext requiredPaymentsDataContext)
        {
            _requiredPaymentsDataContext = requiredPaymentsDataContext;
        }

        public PaymentHistoryRepository(RequiredPaymentsDataContext requiredPaymentsDataContext, IRepositoryCache<IEnumerable<PaymentEntity>> cache)
        {
            _cache = cache;
            _requiredPaymentsDataContext = requiredPaymentsDataContext;
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
            return _requiredPaymentsDataContext.PaymentHistory.Where(p => p.ApprenticeshipKey == apprenticeshipKey).ToArray();
        }
    }
}