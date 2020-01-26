using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
    public interface IMetricsDataContext
    {
        DbSet<SubmissionSummaryModel> SubmissionSummaries { get; }
        DbSet<DataLockCountsModel> DataLockedEarnings { get; }
        DbSet<EarningsModel> Earnings { get; }
        DbSet<RequiredPaymentsModel> RequiredPayments { get; }
        Task Save(SubmissionSummaryModel submissionSummary, CancellationToken cancellationToken);
        Task<decimal> GetAlreadyPaidDataLocksAmount(long ukprn, long jobId, CancellationToken cancellationToken);
    }

    public class MetricsDataContext : DbContext, IMetricsDataContext
    {
        private readonly string connectionString;
        public virtual DbSet<SubmissionSummaryModel> SubmissionSummaries { get; set; }
        public virtual DbSet<DataLockCountsModel> DataLockedEarnings { get; set; }
        public virtual DbSet<EarningsModel> Earnings { get; set; }
        public virtual DbSet<RequiredPaymentsModel> RequiredPayments { get; set; }

        public MetricsDataContext(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("Metrics");
            modelBuilder.ApplyConfiguration(new SubmissionSummaryModelConfiguration());
            modelBuilder.ApplyConfiguration(new DataLockedEarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new EarningsModelConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPaymentsModelConfiguration());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        public async Task<decimal> GetAlreadyPaidDataLocksAmount(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            var sql = @"
	Select
		@result = sum(p.Amount)
		from Payments2.dataLockEventNonPayablePeriod npp
		join Payments2.dataLockEvent dle on npp.DataLockEventId = dle.EventId 
		join Payments2.payment p on dle.ukprn = p.ukprn
			and npp.priceepisodeidentifier = p.priceepisodeidentifier
			and dle.learnerreferencenumber = p.learnerreferencenumber
			and npp.deliveryperiod = p.deliveryperiod
			AND P.TransactionType = npp.TransactionType
		where dle.jobId = @jobid
		and dle.Ukprn = @ukprn
		and npp.Amount <> 0
		and dle.IsPayable = 0	
";
            var result = new SqlParameter("@result", SqlDbType.Decimal) { Direction = ParameterDirection.Output };
            await Database.ExecuteSqlCommandAsync(sql, new[] { new SqlParameter("@jobid", jobId), new SqlParameter("@ukprn", ukprn), result }, cancellationToken);
            return result.Value as decimal? ?? 0;

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
                SubmissionSummaries.Add(submissionSummary);
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