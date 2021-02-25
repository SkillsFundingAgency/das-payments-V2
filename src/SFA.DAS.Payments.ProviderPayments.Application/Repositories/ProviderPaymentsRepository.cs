using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.ProviderPayments.Application.Data;
using SFA.DAS.Payments.ProviderPayments.Application.Exceptions;

namespace SFA.DAS.Payments.ProviderPayments.Application.Repositories
{
    public class ProviderPaymentsRepository : IProviderPaymentsRepository
    {
        private readonly IProviderPaymentsDataContext dataContext;
        private readonly IProviderPaymentsDataContextFactory dataContextFactory;
        private readonly IPaymentLogger logger;

        public ProviderPaymentsRepository(IProviderPaymentsDataContext dataContext, IProviderPaymentsDataContextFactory dataContextFactory, IPaymentLogger logger)
        {
            this.dataContext = dataContext;
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<PaymentModel>> GetMonthEndPayments(CollectionPeriod collectionPeriod, long ukprn,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(p => p.Ukprn == ukprn && 
                            p.CollectionPeriod.Period == collectionPeriod.Period && 
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear)
                .ToListAsync(cancellationToken);
        }

        [Obsolete("Never used (implementation never worked), to be replaced with more suitable solution.")]
        public List<PaymentModelWithRequiredPaymentId> GetMonthEndPayments(CollectionPeriod collectionPeriod, int pageSize,
            int page)
        {
            var payments = dataContext.PaymentsWithRequiredPayments.FromSqlRaw($@"
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
                OFFSET {pageSize * page} ROWS
                FETCH NEXT {pageSize} ROWS ONLY
                "
                )
                .AsNoTracking()
                .ToList();

            return payments;
        }

        public async Task DeleteOldMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSubmittedIlrPayments = dataContext.Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            p.IlrSubmissionDateTime < currentIlrSubmissionDateTime);

            dataContext.Payment.RemoveRange(oldSubmittedIlrPayments);
            await dataContext.SaveChanges(cancellationToken);
        }

        public async Task DeleteCurrentMonthEndPayment(CollectionPeriod collectionPeriod,
            long ukprn,
            DateTime currentIlrSubmissionDateTime,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var oldSubmittedIlrPayments = dataContext.Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            p.IlrSubmissionDateTime == currentIlrSubmissionDateTime);

            dataContext.Payment.RemoveRange(oldSubmittedIlrPayments);
            await dataContext.SaveChanges(cancellationToken);
        }

        public async Task SavePayment(PaymentModel paymentData, CancellationToken cancellationToken)
        {
            await dataContext.Payment.AddAsync(paymentData, cancellationToken);
            await dataContext.SaveChanges(cancellationToken);
        }

        public async Task<List<PaymentModel>> GetMonthEndAct1CompletionPaymentsForProvider(long ukprn, CollectionPeriod collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.ContractType == ContractType.Act1 &&
                            p.TransactionType == TransactionType.Completion)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<long>> GetProvidersWithAct1CompletionPayments(CollectionPeriod collectionPeriod, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dataContext.Payment
                .Where(p => p.CollectionPeriod.AcademicYear == collectionPeriod.AcademicYear &&
                            p.CollectionPeriod.Period == collectionPeriod.Period &&
                            p.ContractType == ContractType.Act1 &&
                            p.TransactionType == TransactionType.Completion)
                .Select(p => p.Ukprn)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task SavePayments(List<PaymentModel> payments, CancellationToken cancellationToken)
        {
            try
            {
                using (var tx = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
                {
                    var bulkConfig = new BulkConfig { SetOutputIdentity = false, BulkCopyTimeout = 60, PreserveInsertOrder = false };
                    await ((DbContext)dataContext).BulkInsertAsync(payments, bulkConfig, null, cancellationToken)
                        .ConfigureAwait(false);
                    await tx.CommitAsync(cancellationToken);
                }
            }
            catch (Exception e)
            {
                if (e.IsUniqueKeyConstraintException())
                    throw new DuplicatePaymentException(e);
                throw;
            }
        }

        public async Task SavePaymentsIndividually(List<PaymentModel> payments, CancellationToken cancellationToken)
        {
            var mainContext = dataContextFactory.Create();
            using (var tx = await ((DbContext)mainContext).Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                foreach (var payment in payments)
                {
                    try
                    {
                        var retryDataContext = dataContextFactory.Create(tx.GetDbTransaction());
                        await retryDataContext.Payment.AddAsync(payment, cancellationToken).ConfigureAwait(false);
                        await retryDataContext.SaveChanges(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (e.IsUniqueKeyConstraintException())
                        {
                            logger.LogInfo($"Discarding duplicate Payment. Event Id: {payment.EventId}, JobId: {payment.JobId}, Learn ref: {payment.LearnerReferenceNumber}");
                            continue;
                        }
                        throw;
                    }
                }
                await tx.CommitAsync(cancellationToken);
            }
        }
    }
}