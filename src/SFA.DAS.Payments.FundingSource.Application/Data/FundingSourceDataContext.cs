using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Data
{
    public interface IFundingSourceDataContext
    {
        DbSet<LevyAccountModel> LevyAccounts { get; }

        DbSet<EmployerProviderPriorityModel>
            EmployerProviderPriorities { get; }

        DbSet<LevyTransactionModel> LevyTransactions { get; }
        Task<int> SaveChanges(CancellationToken cancellationToken);
        IQueryable<LevyTransactionModel> GetEmployerLevyTransactions(long employerAccountId);
        Task SaveBatch(IList<LevyTransactionModel> batch, CancellationToken cancellationToken);
        Task DeletePreviousSubmissions(long jobId, byte collectionPeriod, short academicYear,
            DateTime ilrSubmissionDateTime, long ukprn);
        Task DeleteCurrentSubmissions(long jobId, byte collectionPeriod, short academicYear, long ukprn);
    }


    public class FundingSourceDataContext : DbContext, IFundingSourceDataContext
    {
        protected readonly string connectionString;
        public virtual DbSet<LevyAccountModel> LevyAccounts { get; protected set; }
        public virtual DbSet<EmployerProviderPriorityModel> EmployerProviderPriorities { get; protected set; }
        public virtual DbSet<LevyTransactionModel> LevyTransactions { get; protected set; }

        public async Task<int> SaveChanges(CancellationToken cancellationToken)
        {
            return await SaveChangesAsync(cancellationToken);
        }

        public FundingSourceDataContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new LevyAccountModelConfiguration());
            modelBuilder.ApplyConfiguration(new EmployerProviderPriorityModelConfiguration());
            modelBuilder.ApplyConfiguration(new LevyTransactionModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null) optionsBuilder.UseSqlServer(connectionString);
        }

        public IQueryable<LevyTransactionModel> GetEmployerLevyTransactions(long employerAccountId)
        {
            return LevyTransactions.Where(transaction => transaction.AccountId == employerAccountId);
        }

        public async Task SaveBatch(IList<LevyTransactionModel> batch, CancellationToken cancellationToken)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            await LevyTransactions.AddRangeAsync(batch, cancellationToken).ConfigureAwait(false);
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task DeletePreviousSubmissions(long jobId, byte collectionPeriod, short academicYear, DateTime ilrSubmissionDateTime,
            long ukprn)
        {
            await Database.ExecuteSqlCommandAsync(@"DELETE FROM [Payments2].[FundingSourceLevyTransaction]
                WHERE [AcademicYear] = @academicYear
                AND [CollectionPeriod] = @collectionPeriod
                AND [JobId] != @jobId
                AND [IlrSubmissionDateTime] < @ilrSubmissionDateTime
                AND [Ukprn] = @ukprn", 
            new SqlParameter("academicYear", academicYear),
            new SqlParameter("collectionPeriod", collectionPeriod),
            new SqlParameter("jobId", jobId),
            new SqlParameter("ilrSubmissionDateTime", ilrSubmissionDateTime),
            new SqlParameter("ukprn", ukprn));
        }

        public async Task DeleteCurrentSubmissions(long jobId, byte collectionPeriod, short academicYear, long ukprn)
        {
            await Database.ExecuteSqlCommandAsync(@"DELETE FROM [Payments2].[FundingSourceLevyTransaction]
                WHERE [AcademicYear] = @academicYear
                AND [CollectionPeriod] = @collectionPeriod
                AND [JobId] = @jobId
                AND [Ukprn] = @ukprn", 
            new SqlParameter("academicYear", academicYear),
            new SqlParameter("collectionPeriod", collectionPeriod),
            new SqlParameter("jobId", jobId),
            new SqlParameter("ukprn", ukprn));
        }
    }
}