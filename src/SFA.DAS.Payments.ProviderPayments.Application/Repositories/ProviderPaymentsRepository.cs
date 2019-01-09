using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
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

        public async Task<List<PaymentModel>> GetMonthEndPayments(string collectionPeriodName, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var payments = await paymentsDataContext
                .Payment.Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.Name == collectionPeriodName)
                .ToListAsync(cancellationToken);
            return payments;
        }

        public async Task<List<long>> GetMonthEndUkprns(string collectionPeriodName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await paymentsDataContext
                   .Payment.Where(p => p.CollectionPeriod.Name == collectionPeriodName)
                   .Select(o => o.Ukprn)
                   .ToListAsync(cancellationToken);
        }

        public async Task DeleteOldMonthEndPayment(string collectionPeriodName,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSubmittedIlrPayments = paymentsDataContext.Payment
                .Where(p => p.CollectionPeriod.Name == collectionPeriodName &&
                            p.Ukprn == ukprn &&
                            p.IlrSubmissionDateTime < currentIlrSubmissionDateTime);

            paymentsDataContext.Payment.RemoveRange(oldSubmittedIlrPayments);
            await paymentsDataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SavePayment(PaymentModel paymentData, CancellationToken cancellationToken)
        {
            await paymentsDataContext.Payment.AddAsync(paymentData, cancellationToken);
            await paymentsDataContext.SaveChangesAsync(cancellationToken);
        }
    }
}