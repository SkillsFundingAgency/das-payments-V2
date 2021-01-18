﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Application.Data
{
    public interface IFundingSourceDataContext
    {
        Task<int> SaveChanges(CancellationToken cancellationToken);
        Task<List<LevyTransactionModel>> GetTransactionsToBePaidByEmployer(long employerAccountId, CollectionPeriod collectionPeriod);
        Task DeletePreviousSubmissions(long jobId, byte collectionPeriod, short academicYear, DateTime ilrSubmissionDateTime, long ukprn);
        Task DeleteCurrentSubmissions(long jobId, byte collectionPeriod, short academicYear, long ukprn);
        Task<List<EmployerProviderPriorityModel>> GetEmployerProviderPriorities(long employerAccountId, CancellationToken cancellationToken);
        Task ReplaceEmployerProviderPriorities(long employerAccountId, List<EmployerProviderPriorityModel> paymentPriorityModels, CancellationToken cancellationToken);
    }

    public class FundingSourceDataContext : DbContext, IFundingSourceDataContext
    {
        protected readonly string connectionString;
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

        public FundingSourceDataContext(DbContextOptions<FundingSourceDataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Payments2");
            modelBuilder.ApplyConfiguration(new EmployerProviderPriorityModelConfiguration());
            modelBuilder.ApplyConfiguration(new LevyTransactionModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (connectionString != null) optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<List<LevyTransactionModel>> GetTransactionsToBePaidByEmployer(long employerAccountId, CollectionPeriod collectionPeriod)
        {
            return await LevyTransactions.Where(transaction =>
                transaction.FundingAccountId == employerAccountId 
                && transaction.CollectionPeriod == collectionPeriod.Period 
                && transaction.AcademicYear == collectionPeriod.AcademicYear).ToListAsync();
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

        public async Task<List<EmployerProviderPriorityModel>> GetEmployerProviderPriorities(long employerAccountId, CancellationToken cancellationToken = default(CancellationToken))
        {
            var paymentPriorities = await EmployerProviderPriorities.AsNoTracking()
                .Where(paymentPriority => paymentPriority.EmployerAccountId == employerAccountId)
                .ToListAsync(cancellationToken);

            return paymentPriorities;
        }

        public async Task ReplaceEmployerProviderPriorities(long employerAccountId, List<EmployerProviderPriorityModel> paymentPriorityModels, CancellationToken cancellationToken = default(CancellationToken))
        {
            var previousEmployerPriorities = EmployerProviderPriorities
                .Where(paymentPriority => paymentPriority.EmployerAccountId == employerAccountId);
            EmployerProviderPriorities.RemoveRange(previousEmployerPriorities);

            await EmployerProviderPriorities
                .AddRangeAsync(paymentPriorityModels, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}