using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.EarningEvent
{
    public interface IEarningEventRepository
    {
        Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken);
        Task RemoveFailedSubmissionEvents(long jobId, CancellationToken cancellationToken);
        Task SaveEarningEvents(List<EarningEventModel> earningEvents, CancellationToken cancellationToken);
        Task SaveEarningsIndividually(List<EarningEventModel> earningEvents, CancellationToken cancellationToken);
    }

    public class EarningEventRepository : IEarningEventRepository
    {
        private readonly IAuditDataContext dataContext;
        private readonly IAuditDataContextFactory retryDataContextFactory;
        private readonly IPaymentLogger logger;

        public EarningEventRepository(IAuditDataContext dataContext, IAuditDataContextFactory retryDataContextFactory, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.retryDataContextFactory = retryDataContextFactory ?? throw new ArgumentNullException(nameof(retryDataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task RemovePriorEvents(long ukprn, short academicYear, byte collectionPeriod, DateTime latestIlrSubmissionTime, CancellationToken cancellationToken)
        {
            await dataContext.Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Payments2].[EarningEvent] 
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
                        From [Payments2].[EarningEvent] 
                    Where 
                        JobId = {jobId}", cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task SaveEarningEvents(List<EarningEventModel> earningEvents, CancellationToken cancellationToken)
        {
            using (var tx = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                var bulkConfig = new BulkConfig
                    {SetOutputIdentity = false, BulkCopyTimeout = 60, PreserveInsertOrder = false};
                await ((DbContext) dataContext).BulkInsertAsync(earningEvents, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(earningEvents.SelectMany(earning => earning.Periods).ToList(), bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(earningEvents.SelectMany(earning => earning.PriceEpisodes).ToList(), bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                tx.Commit();
            }
        }

        public async Task SaveEarningsIndividually(List<EarningEventModel> earningEvents, CancellationToken cancellationToken)
        {
            var mainContext = retryDataContextFactory.Create();
            using (var tx = await ((DbContext)mainContext).Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                foreach (var earningEventModel in earningEvents)
                {
                    try
                    {
                        var retryDataContext = retryDataContextFactory.Create(tx.GetDbTransaction());
                        await retryDataContext.EarningEvent.AddAsync(earningEventModel, cancellationToken).ConfigureAwait(false);
                        await retryDataContext.SaveChanges(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (e.IsUniqueKeyConstraintException())
                        {
                            logger.LogInfo($"Discarding duplicate earning event. Event Id: {earningEventModel.EventId}, JobId: {earningEventModel.JobId}, Learn ref: {earningEventModel.LearnerReferenceNumber},  Event Type: {earningEventModel.EventType}");
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