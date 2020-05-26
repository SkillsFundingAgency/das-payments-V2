using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Data.DataLock
{
    public interface IDataLockEventRepository
    {
        Task SaveDataLockEvents(List<DataLockEventModel> dataLockEvents, CancellationToken cancellationToken);
        Task SaveDataLocksIndividually(List<DataLockEventModel> dataLockEvents, CancellationToken cancellationToken);
    }

    public class DataLockEventRepository : IDataLockEventRepository
    {
        private readonly IAuditDataContext dataContext;
        private readonly IAuditDataContextFactory retryDataContextFactory;
        private readonly IPaymentLogger logger;

        public DataLockEventRepository(IAuditDataContext dataContext, IAuditDataContextFactory retryDataContextFactory, IPaymentLogger logger)
        {
            this.dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
            this.retryDataContextFactory = retryDataContextFactory ?? throw new ArgumentNullException(nameof(retryDataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveDataLockEvents(List<DataLockEventModel> dataLockEvents, CancellationToken cancellationToken)
        {
            using (var tx = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                var bulkConfig = new BulkConfig { SetOutputIdentity = false, BulkCopyTimeout = 60, PreserveInsertOrder = false };
                var priceEpisodes = dataLockEvents.SelectMany(earning => earning.PriceEpisodes).ToList();
                var payablePeriods = dataLockEvents.SelectMany(earning => earning.PayablePeriods).ToList();
                var nonPayablePeriods = dataLockEvents.SelectMany(earning => earning.NonPayablePeriods).ToList();
                var failures = dataLockEvents
                    .SelectMany(earning => earning.NonPayablePeriods.SelectMany(npp => npp.Failures)).ToList();


                await ((DbContext)dataContext).BulkInsertAsync(dataLockEvents, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(priceEpisodes, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(payablePeriods, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(nonPayablePeriods, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await ((DbContext)dataContext).BulkInsertAsync(failures, bulkConfig, null, cancellationToken)
                    .ConfigureAwait(false);
                await tx.CommitAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task SaveDataLocksIndividually(List<DataLockEventModel> dataLockEvents, CancellationToken cancellationToken)
        {
            var mainContext = retryDataContextFactory.Create();
            using (var tx = await ((DbContext)mainContext).Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken).ConfigureAwait(false))
            {
                foreach (var dataLockEvent in dataLockEvents)
                {
                    try
                    {
                        var retryDataContext = retryDataContextFactory.Create(tx.GetDbTransaction());
                        await retryDataContext.DataLockEvent.AddAsync(dataLockEvent, cancellationToken).ConfigureAwait(false);
                        await retryDataContext.SaveChanges(cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        if (e.IsUniqueKeyConstraintException())
                        {
                            logger.LogInfo($"Discarding duplicate earning event. Event Id: {dataLockEvent.EventId}, JobId: {dataLockEvent.JobId}, Learn ref: {dataLockEvent.LearnerReferenceNumber},  Event Type: {dataLockEvent.EventType}");
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
