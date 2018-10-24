using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public class ProviderPaymentsRepository : IProviderPaymentsRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public ProviderPaymentsRepository(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext;
        }

        public async Task<List<PaymentDataEntity>> GetMonthEndPayments(short collectionYear,byte collectionPeriodMonth, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await paymentsDataContext
                .Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriodYear == collectionYear &&
                            p.CollectionPeriodMonth == collectionPeriodMonth)
                .ToListAsync(cancellationToken);
            return payments;

        }

        public async Task SavePayment(PaymentDataEntity paymentData, CancellationToken cancellationToken)
        {
            await paymentsDataContext.Payment.AddAsync(paymentData, cancellationToken);
            await paymentsDataContext.SaveChangesAsync(cancellationToken);
        }

    }
}