using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionMetricsRepository
    {
        Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId);
        Task<DataLockTypeAmounts> GetDataLockedEarnings(long ukprn, long jobId);
        Task<decimal> GetDataLockedEarningsTotal(long ukprn, long jobId);
        Task<ContractTypeAmounts> GetHeldBackCompletionPaymentsTotal(long ukprn, long jobId);
        Task<List<TransactionTypeAmounts>> GetRequiredPayments(long ukprn, long jobId);
    }

    public class SubmissionMetricsRepository : ISubmissionMetricsRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;

        public SubmissionMetricsRepository(IPaymentsDataContext paymentsDataContext)
        {
            this.paymentsDataContext = paymentsDataContext ?? throw new ArgumentNullException(nameof(paymentsDataContext));
        }

        public async Task<List<TransactionTypeAmounts>> GetDasEarnings(long ukprn, long jobId)
        {

            var transactionAmounts = await paymentsDataContext.EarningEventPeriod
                .AsNoTracking()
                .Where(ee => ee.EarningEvent.Ukprn == ukprn && ee.EarningEvent.JobId == jobId)
                //.Select(eep => new { eep.Amount, eep.EarningEvent.ContractType, eep.TransactionType})
                .GroupBy(eep => new { eep.EarningEvent.ContractType, eep.TransactionType })
                .Select(group => new
                {
                    ContractType = group.Key.ContractType,
                    TransactionType = group.Key.TransactionType,
                    Amount = group.Sum(x => x.Amount)
                })
                .ToListAsync();

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

        public async Task<DataLockTypeAmounts> GetDataLockedEarnings(long ukprn, long jobId)
        {
            var dataLockAmounts = await paymentsDataContext.DataLockEventNonPayablePeriodFailure
                .AsNoTracking()
                .Where(failure => failure.DataLockEventNonPayablePeriod.DataLockEvent.Ukprn == ukprn &&
                                  failure.DataLockEventNonPayablePeriod.DataLockEvent.JobId == jobId)
                .GroupBy(failure => failure.DataLockFailure)
                .Select(group => new
                {
                    DataLockType = group.Key,
                    Amount = group.Sum(failure => failure.DataLockEventNonPayablePeriod.Amount)
                })
                .ToListAsync();

            return new DataLockTypeAmounts
            {
                DataLock1 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_01)?.Amount ?? 0,
                DataLock2 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_02)?.Amount ?? 0,
                DataLock3 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_03)?.Amount ?? 0,
                DataLock4 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_04)?.Amount ?? 0,
                DataLock5 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_05)?.Amount ?? 0,
                DataLock6 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_06)?.Amount ?? 0,
                DataLock7 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_07)?.Amount ?? 0,
                DataLock8 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_08)?.Amount ?? 0,
                DataLock9 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_09)?.Amount ?? 0,
                DataLock10 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_10)?.Amount ?? 0,
                DataLock11 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_11)?.Amount ?? 0,
                DataLock12 = dataLockAmounts.FirstOrDefault(amount => amount.DataLockType == DataLockErrorCode.DLOCK_12)?.Amount ?? 0,
            };
        }

        public async Task<decimal> GetDataLockedEarningsTotal(long ukprn, long jobId)
        {
            return await paymentsDataContext.DataLockEventNonPayablePeriod
                .Where(period => period.DataLockEvent.Ukprn == ukprn && period.DataLockEvent.JobId == jobId)
                .SumAsync(period => period.Amount);
        }

        public async Task<ContractTypeAmounts> GetHeldBackCompletionPaymentsTotal(long ukprn, long jobId)
        {
            var amounts = await paymentsDataContext.RequiredPaymentEvent
                .AsNoTracking()
                .Where(rp =>
                    rp.Ukprn == ukprn && rp.JobId == jobId && rp.NonPaymentReason != null && rp.NonPaymentReason == NonPaymentReason.InsufficientEmployerContribution)
                .GroupBy(rp => rp.ContractType)
                .Select(group => new
                {
                    ContractType = group.Key, Amount = group.Sum(rp => rp.Amount)
                })
                .ToListAsync()
                .ConfigureAwait(false);

            return new ContractTypeAmounts
            {
                ContractType1 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = amounts.FirstOrDefault(amount => amount.ContractType == ContractType.Act2)?.Amount ?? 0,
            };
        }

        public async Task<List<TransactionTypeAmounts>> GetRequiredPayments(long ukprn, long jobId)
        {
            var transactionAmounts = await paymentsDataContext.RequiredPaymentEvent
                .AsNoTracking()
                .Where(rp => rp.Ukprn == ukprn && rp.JobId == jobId)
                //.Select(eep => new { eep.Amount, eep.EarningEvent.ContractType, eep.TransactionType})
                .GroupBy(rp => new { rp.ContractType, rp.TransactionType })
                .Select(group => new
                {
                    ContractType = group.Key.ContractType,
                    TransactionType = group.Key.TransactionType,
                    Amount = group.Sum(x => x.Amount)
                })
                .ToListAsync();

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

        public async Task SaveSubmissionMetrics(SubmissionSummaryModel submissionSummary)
        {
            throw new NotImplementedException();
        }
    }
}