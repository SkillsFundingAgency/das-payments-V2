using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Polly;
using SFA.DAS.Payments.Application.Data.Configurations;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        Task<List<PeriodEndProviderDataLockTypeCounts>> GetPeriodEndProviderDataLockCounts(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderFundingLineTypeAmounts>> GetDataLockedEarningsTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderFundingLineTypeAmounts>> GetAlreadyPaidDataLockProviderTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts>> GetDataLockedAmountsForForNegativeEarningsLearners(List<long> learnerUlns, short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<List<ProviderContractTypeAmounts>> GetHeldBackCompletionPaymentTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken);
        Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken, IsolationLevel isolationLevel = IsolationLevel.Snapshot);
    }

    public class MetricsQueryDataContext : DbContext, IMetricsQueryDataContext
    {
        public class DataLockCount
        {
            public int Count { get; set; }
            public byte DataLockType { get; set; }
        }

        public class PeriodEndDataLockCount
        {
            public long Ukprn { get; set; }
            public int Count { get; set; }
            public byte DataLockType { get; set; }
        }

        public virtual DbQuery<DataLockCount> DataLockCounts { get; set; }
        public virtual DbQuery<PeriodEndDataLockCount> PeriodEndDataLockCounts { get; set; }
        public virtual DbQuery<ProviderFundingLineTypeAmounts> AlreadyPaidDataLockProviderTotals { get; set; }
        public virtual DbQuery<ProviderFundingLineTypeAmounts> DataLockedEarningsTotals { get; set; }
        public virtual DbQuery<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts> DataLockedEarningsForLearnersWithNegativeDcEarnings { get; set; }
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

        public string GetDataLockedEarningsTotalsSqlQuery(bool shouldGroupByLearner = false) => $@";WITH unGroupedEarnings AS 
                        (Select
	                        dle.ukprn as Ukprn,
							{(shouldGroupByLearner ? "dle.LearnerULn," : "")}
                            CASE WHEN dle.LearningAimFundingLineType IN (
			                        '16 - 18 Apprenticeship(From May 2017) Non - Levy Contract(non - procured)',
			                        '16-18 Apprenticeship Non-Levy Contract (procured)',
			                        '16-18 Apprenticeship (Employer on App Service)'
		                        ) THEN npp.Amount ELSE 0 END AS FundingLineType16To18Amount,
	                        CASE WHEN dle.LearningAimFundingLineType IN (
			                        '19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)',
			                        '19+ Apprenticeship Non-Levy Contract (procured)',
			                        '19+ Apprenticeship (Employer on App Service)'
		                        ) THEN npp.Amount ELSE 0 END AS FundingLineType19PlusAmount,
	                        npp.Amount AS Total
                        from Payments2.dataLockEventNonPayablePeriod npp
                        join Payments2.dataLockEvent dle on npp.DataLockEventId = dle.EventId
                        where 		
	                        dle.jobId in (select DcJobid from Payments2.LatestSuccessfulJobs Where AcademicYear = @academicYear AND CollectionPeriod = @collectionPeriod)
	                        and npp.Amount <> 0
							{(shouldGroupByLearner ? "and dle.LearnerUln in ({0})" : "")}
                        )
                        SELECT Ukprn,
						{(shouldGroupByLearner ? "LearnerUln," : "" )}
	                        SUM(unGroupedEarnings.FundingLineType16To18Amount) AS FundingLineType16To18Amount, 
	                        SUM(unGroupedEarnings.FundingLineType19PlusAmount) AS FundingLineType19PlusAmount,
	                        SUM(unGroupedEarnings.Total) AS Total
	                        FROM unGroupedEarnings
	                        GROUP BY unGroupedEarnings.Ukprn
                            {(shouldGroupByLearner ? ", LearnerUln" : "")}";

        public async Task<List<ProviderFundingLineTypeAmounts>> GetAlreadyPaidDataLockProviderTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            var query =
                from dle in DataLockEvent
                join npp in DataLockEventNonPayablePeriods on dle.EventId equals npp.DataLockEventId
                join p in Payments on new
                    {
                        dle.Ukprn,
                        dle.LearningAimFrameworkCode,
                        dle.LearningAimPathwayCode,
                        dle.LearningAimProgrammeType,
                        dle.LearningAimReference,
                        dle.LearningAimStandardCode,
                        dle.LearnerReferenceNumber,
                        npp.DeliveryPeriod,
                        npp.TransactionType,
                        dle.AcademicYear
                    }
                    equals new
                    {
                        p.Ukprn,
                        p.LearningAimFrameworkCode,
                        p.LearningAimPathwayCode,
                        p.LearningAimProgrammeType,
                        p.LearningAimReference,
                        p.LearningAimStandardCode,
                        p.LearnerReferenceNumber,
                        p.DeliveryPeriod,
                        p.TransactionType,
                        p.CollectionPeriod.AcademicYear
                    }
                where !dle.IsPayable
                      && npp.Amount != 0
                      && dle.JobId == LatestSuccessfulJobs
                          .Where(job => job.AcademicYear == academicYear && job.CollectionPeriod == collectionPeriod)
                          .Select(job => job.DcJobId)
                          .FirstOrDefault()
                      && p.CollectionPeriod.Period < dle.CollectionPeriod
                      && p.ContractType == ContractType.Act1
                      && p.FundingPlatformType != FundingPlatformType.DigitalApprenticeshipService
                select new 
                {
                    Ukprn = dle.Ukprn,
                    FundingLineType16To18Amount = (p.LearningAimFundingLineType ==
                        "16 - 18 Apprenticeship(From May 2017) Non - Levy Contract(non - procured)" ||
                        p.LearningAimFundingLineType == "16-18 Apprenticeship Non-Levy Contract (procured)" ||
                        p.LearningAimFundingLineType == "16-18 Apprenticeship (Employer on App Service)"
                    ) ? p.Amount : 0,
                    FundingLineType19PlusAmount = (p.LearningAimFundingLineType ==
                        "19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)" ||
                        p.LearningAimFundingLineType == "19+ Apprenticeship Non-Levy Contract (procured)" ||
                        p.LearningAimFundingLineType == "19+ Apprenticeship (Employer on App Service)"
                    ) ? p.Amount : 0,
                    Total = p.Amount
                };

            var providerTotals = query
                .GroupBy(g => g.Ukprn)
                .Select(g => new ProviderFundingLineTypeAmounts
                {
                    Ukprn = g.Key,
                    FundingLineType16To18Amount = g.Sum(x => x.FundingLineType16To18Amount),
                    FundingLineType19PlusAmount = g.Sum(x => x.FundingLineType19PlusAmount),
                    Total = g.Sum(x => x.Total)
                }).ToList();

            return providerTotals;
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
            var alreadyPaidDataLocksAmount = (from npp in DataLockEventNonPayablePeriods
                join dle in DataLockEvent on npp.DataLockEventId equals dle.EventId
                join p in Payments on new
                    {
                        dle.Ukprn,
                        dle.LearningAimFrameworkCode,
                        dle.LearningAimPathwayCode,
                        dle.LearningAimProgrammeType,
                        dle.LearningAimReference,
                        dle.LearningAimStandardCode,
                        dle.LearnerReferenceNumber,
                        npp.DeliveryPeriod,
                        npp.TransactionType,
                        dle.AcademicYear
                    }
                    equals new
                    {
                        p.Ukprn,
                        p.LearningAimFrameworkCode,
                        p.LearningAimPathwayCode,
                        p.LearningAimProgrammeType,
                        p.LearningAimReference,
                        p.LearningAimStandardCode,
                        p.LearnerReferenceNumber,
                        p.DeliveryPeriod,
                        p.TransactionType,
                        p.CollectionPeriod.AcademicYear
                    }
                where dle.JobId == jobId
                      && dle.Ukprn == ukprn
                      && npp.Amount != 0
                      && !dle.IsPayable
                      && p.CollectionPeriod.Period < dle.CollectionPeriod
                      && p.ContractType == ContractType.Act1
                      && p.FundingPlatformType == FundingPlatformType.SubmitLearnerData
                select p.Amount).Sum();

            return alreadyPaidDataLocksAmount;
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

        public async Task<List<PeriodEndProviderDataLockTypeCounts>> GetPeriodEndProviderDataLockCounts(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            var dataLockCountByUkprnSql = @"
                select
                    count(*) [Count],
	                a.DataLockFailureId [DataLockType],
                    a.ukprn
                from (
		            select
			            LearnerReferenceNumber,
			            DataLockFailureId,
                        dle.ukprn
		           from Payments2.DataLockEvent dle
		           join Payments2.DataLockEventNonPayablePeriod npp on dle.EventId = npp.DataLockEventId
		           join Payments2.DataLockEventNonPayablePeriodFailures nppf on npp.DataLockEventNonPayablePeriodId = nppf.DataLockEventNonPayablePeriodId
		           where npp.TransactionType in (1,2,3)
			            and (dle.IsPayable = 0)
                        and dle.jobId in (
                            select DcJobId
                            from Payments2.LatestSuccessfulJobs
                            Where AcademicYear = @academicYear
                            and CollectionPeriod = @collectionPeriod)
		           group by dle.LearnerReferenceNumber, nppf.DataLockFailureId, dle.ukprn
                        ) a
                group by
                    a.DataLockFailureId, a.ukprn
                ";

            var providerDataLockCounts = await PeriodEndDataLockCounts
                .FromSql(dataLockCountByUkprnSql, new SqlParameter("@academicYear", academicYear), new SqlParameter("@collectionPeriod", collectionPeriod))
                .ToListAsync(cancellationToken);

            return providerDataLockCounts
                .GroupBy(x => x.Ukprn)
                .Select(dataLockCounts => new PeriodEndProviderDataLockTypeCounts
                {
                    Ukprn = dataLockCounts.Key,
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
                })
                .ToList();
        }

        public async Task<List<ProviderFundingLineTypeAmounts>> GetDataLockedEarningsTotals(short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            var sql = GetDataLockedEarningsTotalsSqlQuery();

            return await DataLockedEarningsTotals.FromSql(sql, new SqlParameter("@academicYear", academicYear), new SqlParameter("@collectionPeriod", collectionPeriod)).ToListAsync(cancellationToken);
        }

        public async Task<List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts>> GetDataLockedAmountsForForNegativeEarningsLearners(List<long> learnerUlns, short academicYear, byte collectionPeriod, CancellationToken cancellationToken)
        {
            var results = new List<ProviderNegativeEarningsLearnerDataLockFundingLineTypeAmounts>();
            var batches = learnerUlns.SplitIntoBatchesOf(2000);

            var sql = GetDataLockedEarningsTotalsSqlQuery(true);

            foreach (var batch in batches)
            {
                var sqlParameters = batch.Select((item, index) => new SqlParameter($"@uln{index}", item)).ToList();
                var sqlParamName = string.Join(", ", sqlParameters.Select(pn => pn.ParameterName));

                sqlParameters.Add(new SqlParameter("@academicYear", academicYear));
                sqlParameters.Add(new SqlParameter("@collectionPeriod", collectionPeriod));

                var batchSqlQuery = string.Format(sql, sqlParamName);

                var queryResult = await DataLockedEarningsForLearnersWithNegativeDcEarnings
                    .FromSql(batchSqlQuery, sqlParameters.ToArray())
                    .ToListAsync(cancellationToken);

                results.AddRange(queryResult);
            }

            return results;
        }

        public async Task<IDbContextTransaction> BeginTransaction(CancellationToken cancellationToken, IsolationLevel isolationLevel = IsolationLevel.Snapshot)
        {
            return await Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        }
    }
}