using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Data;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionMetricsRepository
    {
        Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<DataLockTypeCounts> GetDataLockedEarnings(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<decimal> GetDataLockedEarningsTotal(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<ContractTypeAmounts> GetHeldBackCompletionPaymentsTotal(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<decimal> GetAlreadyPaidDataLockedEarnings(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<List<TransactionTypeAmounts>> GetRequiredPayments(long ukprn, long jobId, CancellationToken cancellationToken);
        Task<ContractTypeAmounts> GetYearToDatePaymentsTotal(long ukprn, short academicYear, byte currentCollectionPeriod, CancellationToken cancellationToken);
        Task SaveSubmissionMetrics(SubmissionSummaryModel submissionSummary, CancellationToken cancellationToken);
        
        Task<SubmissionsSummaryModel> GetSubmissionsSummaryMetrics(long jobId, short academicYear, byte currentCollectionPeriod, CancellationToken cancellationToken);
        Task SaveSubmissionsSummaryMetrics(SubmissionsSummaryModel submissionSummary, CancellationToken cancellationToken);
    }

    public class SubmissionMetricsRepository : ISubmissionMetricsRepository
    {
        private readonly IMetricsPersistenceDataContext persistenceDataContext;
        private readonly IMetricsQueryDataContextFactory metricsQueryDataContextFactory;
        private readonly IPaymentLogger logger;

        private IMetricsQueryDataContext QueryDataContext => metricsQueryDataContextFactory.Create();

        public SubmissionMetricsRepository(IMetricsPersistenceDataContext persistenceDataContext, IMetricsQueryDataContextFactory metricsQueryDataContextFactory, IPaymentLogger logger)
        {
            this.persistenceDataContext = persistenceDataContext ?? throw new ArgumentNullException(nameof(persistenceDataContext));
            this.metricsQueryDataContextFactory = metricsQueryDataContextFactory ?? throw new ArgumentNullException(nameof(metricsQueryDataContextFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            //((IMetricsQueryDataContext)queryDataContext).ConfigureLogging(LogSql, LoggingCategories.SQL);  ////TODO: DO NOT DELETE UNTIL THIS CAN BE CONFIGURED IN CONFIG
        }

        private void LogSql(string sql)
        {
            if (sql.StartsWith("Executing DbCommand"))
                logger.LogDebug($"Sql: {sql}");
        }

        public async Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            var transactionAmounts = await QueryDataContext.EarningEventPeriods
                .AsNoTracking()
                .Where(ee => ee.Amount != 0 && ee.EarningEvent.Ukprn == ukprn && ee.EarningEvent.JobId == jobId)
                .GroupBy(eep => new { eep.EarningEvent.ContractType, eep.TransactionType })
                .Select(group => new
                {
                    ContractType = group.Key.ContractType,
                    TransactionType = group.Key.TransactionType,
                    Amount = group.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);

            return transactionAmounts
                .GroupBy(x => x.ContractType)
                .Select(group => new TransactionTypeAmounts
                {
                    ContractType = group.Key,
                    TransactionType1 = group.Where(x => x.TransactionType == TransactionType.Learning).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType2 = group.Where(x => x.TransactionType == TransactionType.Completion).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType3 = group.Where(x => x.TransactionType == TransactionType.Balancing).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType4 = group.Where(x => x.TransactionType == TransactionType.First16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType5 = group.Where(x => x.TransactionType == TransactionType.First16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType6 = group.Where(x => x.TransactionType == TransactionType.Second16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType7 = group.Where(x => x.TransactionType == TransactionType.Second16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType8 = group.Where(x => x.TransactionType == TransactionType.OnProgramme16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType9 = group.Where(x => x.TransactionType == TransactionType.Completion16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType10 = group.Where(x => x.TransactionType == TransactionType.Balancing16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType11 = group.Where(x => x.TransactionType == TransactionType.FirstDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType12 = group.Where(x => x.TransactionType == TransactionType.SecondDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType13 = group.Where(x => x.TransactionType == TransactionType.OnProgrammeMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType14 = group.Where(x => x.TransactionType == TransactionType.BalancingMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType15 = group.Where(x => x.TransactionType == TransactionType.LearningSupport).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType16 = group.Where(x => x.TransactionType == TransactionType.CareLeaverApprenticePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                })
                .ToList();
        }

        public async Task<DataLockTypeCounts> GetDataLockedEarnings(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            return await QueryDataContext.GetDataLockCounts(ukprn, jobId, cancellationToken).ConfigureAwait(false);
        }

        public async Task<decimal> GetDataLockedEarningsTotal(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            return await QueryDataContext.DataLockEventNonPayablePeriods
                .Where(period => period.Amount != 0 && period.DataLockEvent.Ukprn == ukprn && period.DataLockEvent.JobId == jobId)
                .SumAsync(period => period.Amount, cancellationToken);
        }

        public async Task<decimal> GetAlreadyPaidDataLockedEarnings(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            return await QueryDataContext.GetAlreadyPaidDataLocksAmount(ukprn, jobId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<ContractTypeAmounts> GetHeldBackCompletionPaymentsTotal(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            var amounts = await QueryDataContext.RequiredPaymentEvents
                .AsNoTracking()
                .Where(rp =>
                    rp.Ukprn == ukprn && rp.JobId == jobId && rp.NonPaymentReason != null && rp.NonPaymentReason == NonPaymentReason.InsufficientEmployerContribution)
                .GroupBy(rp => rp.ContractType)
                .Select(group => new
                {
                    ContractType = group.Key,
                    Amount = group.Sum(rp => rp.Amount)
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new ContractTypeAmounts
            {
                ContractType1 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act2)?.Amount ?? 0,
            };
        }

        public async Task<List<TransactionTypeAmounts>> GetRequiredPayments(long ukprn, long jobId, CancellationToken cancellationToken)
        {
            var transactionAmounts = await QueryDataContext.RequiredPaymentEvents
                .AsNoTracking()
                .Where(rp => rp.Ukprn == ukprn && rp.JobId == jobId && rp.NonPaymentReason == null)
                .GroupBy(rp => new { rp.ContractType, rp.TransactionType })
                .Select(group => new
                {
                    ContractType = group.Key.ContractType,
                    TransactionType = group.Key.TransactionType,
                    Amount = group.Sum(x => x.Amount)
                })
                .ToListAsync(cancellationToken);

            return transactionAmounts
                .GroupBy(x => x.ContractType)
                .Select(group => new TransactionTypeAmounts
                {
                    ContractType = group.Key,
                    TransactionType1 = group.Where(x => x.TransactionType == TransactionType.Learning).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType2 = group.Where(x => x.TransactionType == TransactionType.Balancing).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType3 = group.Where(x => x.TransactionType == TransactionType.Completion).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType4 = group.Where(x => x.TransactionType == TransactionType.First16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType5 = group.Where(x => x.TransactionType == TransactionType.First16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType6 = group.Where(x => x.TransactionType == TransactionType.Second16To18EmployerIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType7 = group.Where(x => x.TransactionType == TransactionType.Second16To18ProviderIncentive).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType8 = group.Where(x => x.TransactionType == TransactionType.OnProgramme16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType9 = group.Where(x => x.TransactionType == TransactionType.Completion16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType10 = group.Where(x => x.TransactionType == TransactionType.Balancing16To18FrameworkUplift).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType11 = group.Where(x => x.TransactionType == TransactionType.FirstDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType12 = group.Where(x => x.TransactionType == TransactionType.SecondDisadvantagePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType13 = group.Where(x => x.TransactionType == TransactionType.OnProgrammeMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType14 = group.Where(x => x.TransactionType == TransactionType.BalancingMathsAndEnglish).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType15 = group.Where(x => x.TransactionType == TransactionType.LearningSupport).Sum(x => (decimal?)x.Amount) ?? 0,
                    TransactionType16 = group.Where(x => x.TransactionType == TransactionType.CareLeaverApprenticePayment).Sum(x => (decimal?)x.Amount) ?? 0,
                })
                .ToList();
        }

        public async Task<ContractTypeAmounts> GetYearToDatePaymentsTotal(long ukprn, short academicYear, byte currentCollectionPeriod, CancellationToken cancellationToken)
        {
            var amounts = await QueryDataContext.Payments
                .AsNoTracking()
                .Where(p => p.Ukprn == ukprn &&
                            p.CollectionPeriod.AcademicYear == academicYear &&
                            p.CollectionPeriod.Period < currentCollectionPeriod)
                .GroupBy(p => p.ContractType)
                .Select(g => new { ContractType = g.Key, Amount = g.Sum(p => p.Amount) })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new ContractTypeAmounts
            {
                ContractType1 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act2)?.Amount ?? 0,
            };
        }

        public async Task SaveSubmissionMetrics(SubmissionSummaryModel submissionSummary, CancellationToken cancellationToken)
        {
            await persistenceDataContext.Save(submissionSummary, cancellationToken).ConfigureAwait(false);
        }

        public async Task<SubmissionsSummaryModel> GetSubmissionsSummaryMetrics(long jobId, short academicYear, byte currentCollectionPeriod, CancellationToken cancellationToken)
        {
            decimal GetPercentage(decimal amountA, decimal amountB) => amountA == amountB ? 100 : amountB > 0 ? (amountA / amountB) * 100 : 0;

            var submissions = await persistenceDataContext.SubmissionSummaries.Where(s => s.CollectionPeriod == currentCollectionPeriod && s.AcademicYear == academicYear).ToListAsync(cancellationToken: cancellationToken);

            return new SubmissionsSummaryModel
            {
                JobId = jobId,
                AcademicYear = academicYear,
                CollectionPeriod = currentCollectionPeriod,
                Percentage = (submissions.Sum(s => s.SubmissionMetrics.Total) / submissions.Sum(s => s.DcEarnings.Total)) * 100,
                SubmissionMetrics = new ContractTypeAmountsVerbose
                {
                    ContractType1 = submissions.Sum(s => s.SubmissionMetrics.ContractType1),
                    ContractType2 = submissions.Sum(s => s.SubmissionMetrics.ContractType2),
                    DifferenceContractType1 = submissions.Sum(s => s.SubmissionMetrics.DifferenceContractType1),
                    DifferenceContractType2 = submissions.Sum(s => s.SubmissionMetrics.DifferenceContractType2),
                    PercentageContractType1 = GetPercentage(submissions.Sum(s => s.SubmissionMetrics.PercentageContractType1), submissions.Sum(s => s.DcEarnings.ContractType1)),
                    PercentageContractType2 = GetPercentage(submissions.Sum(s => s.SubmissionMetrics.PercentageContractType2), submissions.Sum(s => s.DcEarnings.ContractType2)),
                },
                DcEarnings = new ContractTypeAmounts
                {
                    ContractType1 = submissions.Sum(s => s.DcEarnings.ContractType1),
                    ContractType2 = submissions.Sum(s => s.DcEarnings.ContractType2),
                },
                DasEarnings = new ContractTypeAmountsVerbose
                {
                    ContractType1 = submissions.Sum(s => s.DasEarnings.ContractType1),
                    ContractType2 = submissions.Sum(s => s.DasEarnings.ContractType2),
                    DifferenceContractType1 = submissions.Sum(s => s.DasEarnings.DifferenceContractType1),
                    DifferenceContractType2 = submissions.Sum(s => s.DasEarnings.DifferenceContractType2),
                    PercentageContractType1 = GetPercentage(submissions.Sum(s => s.DasEarnings.PercentageContractType1), submissions.Sum(s => s.DcEarnings.ContractType1)),
                    PercentageContractType2 = GetPercentage(submissions.Sum(s => s.DasEarnings.PercentageContractType2), submissions.Sum(s => s.DcEarnings.ContractType2)),
                },
                AdjustedDataLockedEarnings = submissions.Sum(s => s.DataLockedEarnings),
                TotalDataLockedEarnings = submissions.Sum(s => s.TotalDataLockedEarnings),
                AlreadyPaidDataLockedEarnings = submissions.Sum(s => s.AlreadyPaidDataLockedEarnings),
                HeldBackCompletionPayments = new ContractTypeAmounts
                {
                    ContractType1 = submissions.Sum(s => s.HeldBackCompletionPayments.ContractType1),
                    ContractType2 = submissions.Sum(s => s.HeldBackCompletionPayments.ContractType2),
                },
                RequiredPayments = new ContractTypeAmounts
                {
                    ContractType1 = submissions.Sum(s => s.RequiredPayments.ContractType1),
                    ContractType2 = submissions.Sum(s => s.RequiredPayments.ContractType2),
                },
                YearToDatePayments = new ContractTypeAmounts
                {
                    ContractType1 = submissions.Sum(s => s.YearToDatePayments.ContractType1),
                    ContractType2 = submissions.Sum(s => s.YearToDatePayments.ContractType2),
                }
            };
        }

        public async Task SaveSubmissionsSummaryMetrics(SubmissionsSummaryModel submissionsSummary, CancellationToken cancellationToken)
        {
           await persistenceDataContext.SaveSubmissionsSummaryMetrics(submissionsSummary, cancellationToken);
        }
    }
}