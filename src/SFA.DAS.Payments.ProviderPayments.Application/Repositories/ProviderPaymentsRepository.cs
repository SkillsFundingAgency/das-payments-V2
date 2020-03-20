using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.ProviderPayments.Application.Data;

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

        public List<PaymentModelWithRequiredPaymentId> GetMonthEndPayments(CollectionPeriod collectionPeriod, int pageSize,
            int page)
        {
            var payments = paymentsDataContext.PaymentsWithRequiredPayments.FromSql($@"
                SELECT R.EventId [RequiredPaymentEventId], P.*, E.LearningAimSequenceNumber, R.Amount [AmountDue]
                  FROM [Payments2].[Payment] P
                JOIN Payments2.FundingSourceEvent F
	                ON F.EventId = P.FundingSourceEventId
                JOIN Payments2.RequiredPaymentEvent R
	                ON R.EventId = F.RequiredPaymentEventId
                JOIN Payments2.EarningEvent E
	                ON E.EventId = P.EarningEventId
                WHERE P.AcademicYear = {collectionPeriod.AcademicYear}
                    AND P.CollectionPeriod = {collectionPeriod.Period}
                ORDER BY P.Id
                OFFSET {pageSize*page} ROWS
                FETCH NEXT {pageSize} ROWS ONLY
                "
                )
                .AsNoTracking()
                .ToList();

            return payments;
        }

        public async Task<List<long>> GetMonthEndProviders(CollectionPeriod collectionPeriod,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await paymentsDataContext
                   .Payment.Where(p =>
                        p.CollectionPeriod.Period == collectionPeriod.Period &&
                        p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                   .Select(o => o.Ukprn)
                   .Distinct()
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

        public async Task DeleteCurrentMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSubmittedIlrPayments = paymentsDataContext.Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            p.IlrSubmissionDateTime == currentIlrSubmissionDateTime);

            paymentsDataContext.Payment.RemoveRange(oldSubmittedIlrPayments);
            await paymentsDataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SavePayment(PaymentModel paymentData, CancellationToken cancellationToken)
        {
            await paymentsDataContext.Payment.AddAsync(paymentData, cancellationToken);
            await paymentsDataContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndAct1CompletionPayments(CollectionPeriod collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await paymentsDataContext
                         .Payment.Where(p =>
                                            p.CollectionPeriod.Period == collectionPeriod.Period 
                                         && p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear 
                                         && p.TransactionType == TransactionType.Completion 
                                         && p.ContractType == ContractType.Act1)
                         .ToListAsync(cancellationToken);
        }
    }
}