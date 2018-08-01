using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.PaymentsDue.Application.Data;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Application.Repositories
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private DedsContext _dedsContext;

        public PaymentHistoryRepository(DedsContext dedsContext)
        {
            _dedsContext = dedsContext;
        }

        public PaymentHistoryRepository(IRepositoryCache<IEnumerable<PaymentEntity>> cache, DedsContext dedsContext)
        {
            _dedsContext = dedsContext;
        }

        public Task<IEnumerable<Payment>> GetPaymentHistory(string apprenticeshipKey)
        {
            throw new NotImplementedException();
        }
    }
}