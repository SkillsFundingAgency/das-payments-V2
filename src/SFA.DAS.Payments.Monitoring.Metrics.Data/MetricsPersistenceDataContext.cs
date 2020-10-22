using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{

    public interface IMetricsPersistenceDataContext
    {
        DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }
        DbSet<CollectionPeriodToleranceModel> CollectionPeriodTolerances { get; set; }

        Task Save(SubmissionSummaryModel submissionSummary, CancellationToken cancellationToken);
        Task SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries, PeriodEndSummaryModel overallPeriodEndSummary, CancellationToken cancellationToken);
        Task SaveSubmissionsSummaryMetrics(SubmissionsSummaryModel submissionsSummary, CancellationToken cancellationToken);
    }

    public class MetricsPersistenceDataContext : DbContext, IMetricsPersistenceDataContext
    {
        public virtual DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }
        public virtual DbSet<PeriodEndSummaryModel> PeriodEndSummaries { get; set; }
        public virtual DbSet<ProviderPeriodEndSummaryModel> ProviderPeriodEndSummaries { get; set; }
        public virtual DbSet<ProviderPaymentTransactionModel> ProviderPaymentTransactions { get; set; }
        public virtual DbSet<ProviderPaymentFundingSourceModel> ProviderPaymentFundingSources { get; set; }
        public virtual DbSet<CollectionPeriodToleranceModel> CollectionPeriodTolerances { get; set; }

        public virtual DbSet<SubmissionsSummaryModel> SubmissionsSummaries { get; set; }

        public MetricsPersistenceDataContext(DbContextOptions contextOptions) : base(contextOptions)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Metrics");
            modelBuilder.ApplyConfiguration(new SubmissionSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockedEarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentsModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPeriodEndSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new PeriodEndSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPaymentTransactionModelConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderPaymentFundingSourceModelConfiguration());
            modelBuilder.ApplyConfiguration(new SubmissionsSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new CollectionPeriodToleranceModelConfiguration());
        }

        public async Task Save(SubmissionSummaryModel submissionSummary, CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Metrics].[SubmissionSummary] 
                    Where 
                        Ukprn = {submissionSummary.Ukprn}
                        And AcademicYear = {submissionSummary.AcademicYear}
                        And CollectionPeriod = {submissionSummary.CollectionPeriod}
                    "
                    , cancellationToken);
                await SubmissionSummaries.AddAsync(submissionSummary, cancellationToken);
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SaveProviderSummaries(List<ProviderPeriodEndSummaryModel> providerSummaries, PeriodEndSummaryModel overallPeriodEndSummary, CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Metrics].[PeriodEndSummary] 
                    Where 
                        AcademicYear = {overallPeriodEndSummary.AcademicYear}
                        And CollectionPeriod = {overallPeriodEndSummary.CollectionPeriod}
                    "
                    , cancellationToken);

                await PeriodEndSummaries.AddAsync(overallPeriodEndSummary, cancellationToken);

                await Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Metrics].[ProviderPeriodEndSummary] 
                    Where 
                        AcademicYear = {overallPeriodEndSummary.AcademicYear}
                        And CollectionPeriod = {overallPeriodEndSummary.CollectionPeriod}
                    "
                    , cancellationToken);
                await ProviderPeriodEndSummaries.AddRangeAsync(providerSummaries, cancellationToken);

                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SaveSubmissionsSummaryMetrics(SubmissionsSummaryModel submissionsSummary, CancellationToken cancellationToken)
        {
            var transaction = await Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await Database.ExecuteSqlCommandAsync($@"
                    Delete 
                        From [Metrics].[SubmissionsSummary] 
                    Where 
                        AcademicYear = {submissionsSummary.AcademicYear}
                        And CollectionPeriod = {submissionsSummary.CollectionPeriod}
                    "
                    , cancellationToken);

                await SubmissionsSummaries.AddAsync(submissionsSummary, cancellationToken);

                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}