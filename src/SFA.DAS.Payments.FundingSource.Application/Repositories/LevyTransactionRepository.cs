using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Repositories
{
    public interface ILevyTransactionRepository
    {
        Task SaveLevyTransactions(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken);

        Task SaveLevyTransactionsIndividually(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken);
    }

    public class LevyTransactionRepository : ILevyTransactionRepository
    {
        private readonly IPaymentLogger logger;
        private readonly IFundingSourceDataContextFactory dataContextFactory;

        public LevyTransactionRepository(IFundingSourceDataContextFactory dataContextFactory, IPaymentLogger logger)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveLevyTransactions(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken)
        {
            using (var context = (FundingSourceDataContext) dataContextFactory.Create())
            {
                context.ChangeTracker.AutoDetectChangesEnabled = false;
                await context.LevyTransactions.AddRangeAsync(levyTransactions, cancellationToken).ConfigureAwait(false);
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task SaveLevyTransactionsIndividually(IList<LevyTransactionModel> levyTransactions, CancellationToken cancellationToken)
        {
            using (var mainContext = (FundingSourceDataContext) dataContextFactory.Create())
            {
                using (var mainTransaction = await mainContext.Database
                    .BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken)
                    .ConfigureAwait(false))
                {
                    foreach (var model in levyTransactions)
                    {
                        try
                        {
                            var context = (FundingSourceDataContext) dataContextFactory.Create(mainTransaction.GetDbTransaction());
                            await context.LevyTransactions.AddAsync(model, cancellationToken).ConfigureAwait(false);
                            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                        }
                        catch (Exception e)
                        {
                            if (e.IsUniqueKeyConstraintException())
                            {
                                logger.LogInfo($"Discarding duplicate LevyTransaction. JobId: {model.JobId}, Learn ref: {model.LearnerReferenceNumber}");
                                continue;
                            }
                            throw;
                        }
                    }
                    await mainTransaction.CommitAsync(cancellationToken);
                }
            }
        }

    }
}