using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Monitoring.Metrics.Data.Configuration;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
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
        Task<DataLockTypeCounts> GetDataLockCounts(long ukprn, long jobId, CancellationToken cancellationToken);
    }

    public class MetricsDataContext : DbContext, IMetricsDataContext
    {
        public class DataLockCount
        {
            public int Count { get; set; }
            public byte DataLockType { get; set; }
        }
        public virtual DbQuery<DataLockCount> DataLockCounts { get; set; }

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
	    AND dle.LearningAimFrameworkCode = P.LearningAimFrameworkCode
	    AND dle.LearningAimPathwayCode = P.LearningAimPathwayCode
	    AND dle.LearningAimProgrammeType = P.LearningAimProgrammeType
	    AND dle.LearningAimReference = P.LearningAimReference
	    AND dle.LearningAimStandardCode = P.LearningAimStandardCode
	    and dle.learnerreferencenumber = p.learnerreferencenumber
	    and npp.deliveryperiod = p.deliveryperiod
	    AND npp.TransactionType = P.TransactionType
    where 		
	    dle.jobId = @jobid
	    and dle.Ukprn = @ukprn
	    and npp.Amount <> 0
	    and dle.IsPayable = 0	
	    and p.collectionperiod < dle.CollectionPeriod
";
            var result = new SqlParameter("@result", SqlDbType.Decimal) { Direction = ParameterDirection.Output };
            await Database.ExecuteSqlCommandAsync(sql, new[] { new SqlParameter("@jobid", jobId), new SqlParameter("@ukprn", ukprn), result }, cancellationToken);
            return result.Value as decimal? ?? 0;
        }



        public async Task<DataLockTypeCounts> GetDataLockCounts(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            var sql = @"
select 
	count(*) [Count],
	a.DataLockFailureId [DataLockType]
	from (
			select 	
				LearnerReferenceNumber, 
				DataLockFailureId
				from Payments2.DataLockEvent dle
				join Payments2.DataLockEventNonPayablePeriod npp on dle.EventId = npp.DataLockEventId
				join Payments2.DataLockEventNonPayablePeriodFailures nppf on npp.DataLockEventNonPayablePeriodId = nppf.DataLockEventNonPayablePeriodId
				where dle.Ukprn = @ukprn
				and JobId = @jobId
				and npp.TransactionType in (1,2,3)
				and (dle.IsPayable = 0)
				group by dle.LearnerReferenceNumber, nppf.DataLockFailureId
				) a
			group by a.DataLockFailureId
";
            var dataLockCounts = await DataLockCounts.FromSql(sql, new SqlParameter("@jobId", jobId), new SqlParameter("@ukprn", ukprn))
                .ToListAsync(cancellationToken);
            return new DataLockTypeCounts
            {
                DataLock1 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_01)?.Count ?? 0,
                DataLock2 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_02)?.Count ?? 0,
                DataLock3 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_03)?.Count ?? 0,
                DataLock4 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_04)?.Count ?? 0,
                DataLock5 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_05)?.Count ?? 0,
                DataLock6 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_06)?.Count ?? 0,
                DataLock7 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_07)?.Count ?? 0,
                DataLock8 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_08)?.Count ?? 0,
                DataLock9 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_09)?.Count ?? 0,
                DataLock10 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_10)?.Count ?? 0,
                DataLock11 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_11)?.Count ?? 0,
                DataLock12 = dataLockCounts.FirstOrDefault(amount => amount.DataLockType == (byte)DataLockErrorCode.DLOCK_12)?.Count ?? 0,
            };
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