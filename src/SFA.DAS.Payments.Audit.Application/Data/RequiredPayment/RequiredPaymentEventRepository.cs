using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.RequiredPayment
{
    public interface IRequiredPaymentEventRepository
    {
        Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime,
            CancellationToken cancellationToken);

        Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken);

        Task SaveRequiredPaymentEvents(List<RequiredPaymentEventModel> requiredPayments,
            CancellationToken cancellationToken);

        Task SaveRequiredPaymentEventsIndividually(List<RequiredPaymentEventModel> requiredPayments,
            CancellationToken cancellationToken);
    }

    public class RequiredPaymentEventRepository : IRequiredPaymentEventRepository
    {
        private readonly IAuditDataContext dataContext;
        private readonly IAuditDataContextFactory retryDataContextFactory;
        private readonly IPaymentLogger logger;

        public RequiredPaymentEventRepository(IAuditDataContext dataContext, IAuditDataContextFactory retryDataContextFactory, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.retryDataContextFactory = retryDataContextFactory ?? throw new ArgumentNullException(nameof(retryDataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[RequiredPaymentEvent] 
                    Where 
                        Ukprn = {ukprn}
                        And AcademicYear = {academicYear}
                        And CollectionPeriod = {collectionPeriod}
                        And IlrSubmissionDateTime < {latestIlrSubmissionTime}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[RequiredPaymentEvent] 
                    Where 
                        JobId = {jobId}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SaveRequiredPaymentEvents(List<RequiredPaymentEventModel> requiredPayments, CancellationToken cancellationToken)
        {
            using (var tx = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                var bulkConfig = new BulkConfig
                { SetOutputIdentity = false, BulkCopyTimeout = 60, PreserveInsertOrder = false };
                await ((DbContext)dataContext).BulkInsertAsync(requiredPayments, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                tx.Commit();
            }
        }

        public async Task SaveRequiredPaymentEventsIndividually(List<RequiredPaymentEventModel> requiredPayments, CancellationToken cancellationToken)
        {
            var mainContext = retryDataContextFactory.Create();
            using (var tx = await ((DbContext)mainContext).Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                foreach (var requiredPaymentEvent in requiredPayments)
                {
                    try
                    {
                        var retryDataContext = retryDataContextFactory.Create(tx.GetDbTransaction());
                        await retryDataContext.RequiredPayment.AddAsync(requiredPaymentEvent, cancellationToken).ConfigureAwait(false);
                        await retryDataContext.SaveChanges(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (e.IsUniqueKeyConstraintException())
                        {
                            logger.LogInfo($"Discarding duplicate required payment event. Event Id: {requiredPaymentEvent.EventId}, JobId: {requiredPaymentEvent.JobId}, Learn ref: {requiredPaymentEvent.LearnerReferenceNumber}");
                            continue;
                        }
                        throw;
                    }
                }
                tx.Commit();
            }
        }
    }
}