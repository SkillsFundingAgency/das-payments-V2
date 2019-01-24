using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
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

        public async Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await paymentsDataContext.Payment
                .Where(p => p.Ukprn == ukprn && 
                            p.CollectionPeriod.Period == collectionPeriod.Period && 
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<long>> GetMonthEndProviders(CollectionPeriod collectionPeriod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await paymentsDataContext
                   .Payment.Where(p =>
                        p.CollectionPeriod.Period == collectionPeriod.Period &&
                        p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                   .Select(o => o.Ukprn)
                   .ToListAsync(cancellationToken);
        }

        public async Task DeleteOldMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSubmittedIlrPayments = paymentsDataContext.Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
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