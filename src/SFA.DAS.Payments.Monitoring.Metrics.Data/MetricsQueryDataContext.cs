using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;

namespace SFA.DAS.Payments.Monitoring.Metrics.Data
{
	public interface IMetricsQueryDataContext
	{
		DbSet<EarningEventPeriodModel> EarningEventPeriods { get; }
		DbSet<DataLockEventNonPayablePeriodModel> DataLockEventNonPayablePeriods { get; }
		DbSet<RequiredPaymentEventModel> RequiredPaymentEvents { get; }
		DbSet<PaymentModel> Payments { get; }
		DbSet<LatestSuccessfulJobModel> LatestSuccessfulJobs { get; }
		Task<decimal> GetAlreadyPaidDataLocksAmount(long ukprn, long jobId, CancellationToken cancellationToken);
		Task<DataLockTypeCounts> GetDataLockCounts(long ukprn, long jobId, CancellationToken cancellationToken);
		Task<List<ProviderTotal>> GetDataLockedEarningsTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
		Task<List<ProviderTotal>> GetAlreadyPaidDataLockProviderTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
		Task<List<ProviderContractTypeAmounts>> GetHeldBackCompletionPaymentTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
	}

	public class MetricsQueryDataContext : DbContext, IMetricsQueryDataContext
	{
		public class DataLockCount
		{
			public int Count { get; set; }
			public byte DataLockType { get; set; }
		}

		public virtual DbQuery<DataLockCount> DataLockCounts { get; set; }
		public virtual DbQuery<ProviderTotal> AlreadyPaidDataLockProviderTotals { get; set; }
		public virtual DbSet<LatestSuccessfulJobModel> LatestSuccessfulJobs { get; set; }
		public virtual DbSet<EarningEventModel> EarningEvent { get; protected set; }
		public virtual DbSet<EarningEventPeriodModel> EarningEventPeriods { get; protected set; }
		public virtual DbSet<DataLockEventModel> DataLockEvent { get; set; }
		public virtual DbSet<DataLockEventNonPayablePeriodModel> DataLockEventNonPayablePeriods { get; set; }
		public virtual DbSet<RequiredPaymentEventModel> RequiredPaymentEvents { get; set; }
		public virtual DbSet<PaymentModel> Payments { get; set; }

		public MetricsQueryDataContext(DbContextOptions contextOptions) : base(contextOptions)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.HasDefaultSchema("Payments2");
			modelBuilder.ApplyConfiguration(new EarningEventModelConfiguration());
			modelBuilder.ApplyConfiguration(new EarningEventPeriodModelConfiguration());
			modelBuilder.ApplyConfiguration(new DataLockEventModelConfiguration());
			modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodModelConfiguration());
			modelBuilder.ApplyConfiguration(new DataLockEventNonPayablePeriodFailureModelConfiguration());
			modelBuilder.ApplyConfiguration(new RequiredPaymentEventModelConfiguration());
			modelBuilder.ApplyConfiguration(new PaymentModelConfiguration());
			modelBuilder.ApplyConfiguration(new LatestSuccessfulJobModelConfiguration());
		}

		public async Task<List<ProviderTotal>> GetAlreadyPaidDataLockProviderTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
		{
			var sql =
		  @"Select
		        dle.ukprn as Ukprn,
		        sum(p.Amount) as TotalAmount
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
                AND dle.AcademicYear = p.AcademicYear
            where 		
	            dle.jobId in (select DcJobid from Payments2.LatestSuccessfulJobs Where AcademicYear = @academicYear AND CollectionPeriod = @collectionPeriod)
	            and npp.Amount <> 0
	            and dle.IsPayable = 0	
	            and p.collectionperiod < dle.CollectionPeriod
	        group by 
		        dle.ukprn";

			return await AlreadyPaidDataLockProviderTotals.FromSql(sql, new SqlParameter("@academicYear", academicYear), new SqlParameter("@collectionPeriod", collectionPeriod))
				.ToListAsync(cancellationToken);
		}

		public async Task<List<ProviderContractTypeAmounts>> GetHeldBackCompletionPaymentTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
		{
			var latestSuccessfulJobIds = LatestSuccessfulJobs.AsNoTracking()
															.Where(j => j.AcademicYear == academicYear &&
																		j.CollectionPeriod == collectionPeriod)
															.Select(x => x.DcJobId);

			var providerMetrics = await RequiredPaymentEvents
				.AsNoTracking()
				.Where(rp => latestSuccessfulJobIds.Contains(rp.JobId) &&
							 rp.NonPaymentReason != null &&
							 rp.NonPaymentReason == NonPaymentReason.InsufficientEmployerContribution)
				.GroupBy(rp => new { rp.Ukprn, rp.ContractType })
				.Select(group => new
				{
					group.Key.Ukprn,
					group.Key.ContractType,
					Amount = group.Sum(requiredPaymentInGroup => requiredPaymentInGroup.Amount)
				})
				.ToListAsync(cancellationToken)
				.ConfigureAwait(false);

			var uniqueUkprns = providerMetrics.Select(x => x.Ukprn).Distinct();

			var results = new List<ProviderContractTypeAmounts>();

			foreach (var ukprn in uniqueUkprns)
			{
				results.Add(new ProviderContractTypeAmounts
				{
					Ukprn = ukprn,
					ContractType1 = providerMetrics.FirstOrDefault(providerMetric =>
						providerMetric.ContractType == ContractType.Act1 &&
						providerMetric.Ukprn == ukprn)?.Amount ?? 0,
					ContractType2 = providerMetrics.FirstOrDefault(providerMetric =>
						providerMetric.ContractType == ContractType.Act2 &&
						providerMetric.Ukprn == ukprn)?.Amount ?? 0,
				});
			}

			return results;
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
				    AND npp.TransactionType = p.TransactionType
                    AND dle.AcademicYear = p.AcademicYear
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
			    group by
                    a.DataLockFailureId
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

		public async Task<List<ProviderTotal>> GetDataLockedEarningsTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
		{
			var latestSuccessfulJobIds = LatestSuccessfulJobs.AsNoTracking()
															.Where(j => j.AcademicYear == academicYear && j.CollectionPeriod == collectionPeriod)
															.Select(x => x.DcJobId);
			return await DataLockEventNonPayablePeriods
				.Where(period => period.Amount != 0 && latestSuccessfulJobIds.Contains(period.DataLockEvent.JobId))
				.GroupBy(x => x.DataLockEvent.Ukprn)
				.Select(x => new ProviderTotal() { Ukprn = x.Key, TotalAmount = x.Sum(period => period.Amount) })
				.ToListAsync(cancellationToken);
		}
	}
}