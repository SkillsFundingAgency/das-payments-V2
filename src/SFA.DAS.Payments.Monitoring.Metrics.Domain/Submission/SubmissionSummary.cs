using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission
{
    public interface ISubmissionSummary
    {
        void AddEarnings(List<TransactionTypeAmounts> dcEarningTransactionTypeAmounts, List<TransactionTypeAmounts> dasEarningTransactionTypeAmounts);
        void AddDataLockTypeCounts(decimal total, DataLockTypeCounts dataLockedCounts, decimal alreadyPaidDataLockedEarnings);
        void AddHeldBackCompletionPayments(ContractTypeAmounts heldBackCompletionPaymentAmounts);
        void AddRequiredPayments(List<TransactionTypeAmounts> requiredPaymentAmounts);
        void AddYearToDatePaymentTotals(ContractTypeAmounts yearToDateAmounts);
        SubmissionSummaryModel GetMetrics();
    }

    public class SubmissionSummary : ISubmissionSummary
    {
        public long Ukprn { get; }
        public long JobId { get; }
        public byte CollectionPeriod { get; }
        public short AcademicYear { get; }
        private readonly List<TransactionTypeAmounts> dcEarnings;
        private readonly List<TransactionTypeAmounts> dasEarnings;
        private DataLockTypeCounts dataLocked;
        private decimal actualTotalDataLocked;
        private decimal alreadyPaidDataLocked;
        private ContractTypeAmounts heldBackCompletionPayments;
        private List<TransactionTypeAmounts> requiredPayments;
        private ContractTypeAmounts yearToDatePayments;

        public SubmissionSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            dcEarnings = new List<TransactionTypeAmounts>();
            dasEarnings = new List<TransactionTypeAmounts>();
            dataLocked = new DataLockTypeCounts();
            requiredPayments = new List<TransactionTypeAmounts>();
            heldBackCompletionPayments = new ContractTypeAmounts();
            yearToDatePayments = new ContractTypeAmounts();
        }

        public virtual void AddEarnings(List<TransactionTypeAmounts> dcEarningTransactionTypeAmounts,
            List<TransactionTypeAmounts> dasEarningTransactionTypeAmounts)
        {
            dcEarnings.Clear();
            dcEarnings.AddRange(dcEarningTransactionTypeAmounts);
            dasEarnings.Clear();
            dasEarnings.AddRange(dasEarningTransactionTypeAmounts);
        }

        public virtual void AddDataLockTypeCounts(decimal total, DataLockTypeCounts dataLockedCounts,
            decimal alreadyPaidDataLockedEarnings)
        {
            actualTotalDataLocked = total;
            dataLocked = dataLockedCounts ?? throw new ArgumentNullException(nameof(dataLockedCounts));
            alreadyPaidDataLocked = alreadyPaidDataLockedEarnings;
        }

        public virtual void AddHeldBackCompletionPayments(ContractTypeAmounts heldBackCompletionPaymentAmounts)
        {
            heldBackCompletionPayments = heldBackCompletionPaymentAmounts ??
                                         throw new ArgumentNullException(nameof(heldBackCompletionPaymentAmounts));
        }

        public virtual void AddRequiredPayments(List<TransactionTypeAmounts> requiredPaymentAmounts)
        {
            requiredPayments = requiredPaymentAmounts ??
                               throw new ArgumentNullException(nameof(requiredPaymentAmounts));
        }

        public virtual void AddYearToDatePaymentTotals(ContractTypeAmounts yearToDateAmounts)
        {
            yearToDatePayments = yearToDateAmounts ?? throw new ArgumentNullException(nameof(yearToDateAmounts));
        }

        public virtual SubmissionSummaryModel GetMetrics()
        {
            var result = new SubmissionSummaryModel
            {
                CollectionPeriod = CollectionPeriod,
                AcademicYear = AcademicYear,
                JobId = JobId,
                Ukprn = Ukprn,
                DcEarnings = GetDcEarnings(),
                AlreadyPaidDataLockedEarnings = alreadyPaidDataLocked,
                TotalDataLockedEarnings = actualTotalDataLocked,
                AdjustedDataLockedEarnings = actualTotalDataLocked - alreadyPaidDataLocked,
                DataLockMetrics = new List<DataLockCountsModel> {new DataLockCountsModel {Amounts = dataLocked}},
                HeldBackCompletionPayments = heldBackCompletionPayments,
                YearToDatePayments = yearToDatePayments,
                RequiredPayments = GetRequiredPayments(),
                RequiredPaymentsMetrics = GetRequiredPaymentsMetrics(),
            };
            result.DasEarnings = GetDasEarnings(result.DcEarnings.ContractType1, result.DcEarnings.ContractType2);
            result.EarningsMetrics = new List<EarningsModel>();
            result.EarningsMetrics.AddRange(dcEarnings.Select(earning => new EarningsModel
            {
                EarningsType = EarningsType.Dc,
                Amounts = earning
            }));
            result.EarningsMetrics.AddRange(dasEarnings.Select(earning => new EarningsModel
            {
                EarningsType = EarningsType.Das,
                Amounts = earning
            }));
            result.SubmissionMetrics = GetSubmissionMetrics(result);
            result.Percentage = result.SubmissionMetrics.Percentage;
            return result;
        }

        private ContractTypeAmountsVerbose GetDcEarnings()
        {
            var contractTypes = dcEarnings.GroupBy(earning => earning.ContractType)
                .Select(g => new {ContractType = g.Key, Amount = g.Sum(x => x.Total)})
                .ToList();
            var result = new ContractTypeAmountsVerbose
            {
                ContractType1 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Amount ?? 0
            };

            return result;
        }

        private ContractTypeAmountsVerbose GetDasEarnings(decimal dcContractTpe1, decimal dcContractTpe2)
        {
            var contractTypes = dasEarnings.GroupBy(earning => earning.ContractType)
                    .Select(g => new {ContractType = g.Key, Amount = g.Sum(x => x.Total)})
                    .ToList()
                ;
            var result = new ContractTypeAmountsVerbose
            {
                ContractType1 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Amount ?? 0,
            };
            result.DifferenceContractType1 = result.ContractType1 - dcContractTpe1;
            result.DifferenceContractType2 = result.ContractType2 - dcContractTpe2;
            result.PercentageContractType1 = Helpers.GetPercentage(result.ContractType1, dcContractTpe1);
            result.PercentageContractType2 = Helpers.GetPercentage(result.ContractType2, dcContractTpe2);
            result.Percentage = Helpers.GetPercentage(result.Total, dcContractTpe1 + dcContractTpe2);
            return result;
        }

        private ContractTypeAmounts GetRequiredPayments()
        {
            return new ContractTypeAmounts
            {
                ContractType1 = requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Total ?? 0,
                ContractType2 = requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Total ?? 0,
            };
        }

        private List<RequiredPaymentsModel> GetRequiredPaymentsMetrics()
        {
            return requiredPayments.Select(amounts => new RequiredPaymentsModel
            {
                Amounts = amounts
            }).ToList();
        }

        private ContractTypeAmountsVerbose GetSubmissionMetrics(SubmissionSummaryModel submissionSummary)
        {
            var submissionMetrics = new ContractTypeAmountsVerbose
            {
                ContractType1 = submissionSummary.YearToDatePayments.ContractType1 +
                                submissionSummary.RequiredPayments.ContractType1 +
                                submissionSummary.AdjustedDataLockedEarnings +
                                submissionSummary.HeldBackCompletionPayments.ContractType1,
                ContractType2 = submissionSummary.YearToDatePayments.ContractType2 +
                                submissionSummary.RequiredPayments.ContractType2 +
                                submissionSummary.HeldBackCompletionPayments.ContractType2
            };
            submissionMetrics.DifferenceContractType1 =
                submissionMetrics.ContractType1 - submissionSummary.DcEarnings.ContractType1;
            submissionMetrics.DifferenceContractType2 =
                submissionMetrics.ContractType2 - submissionSummary.DcEarnings.ContractType2;
            submissionMetrics.PercentageContractType1 = Helpers.GetPercentage(submissionMetrics.ContractType1,
                submissionSummary.DcEarnings.ContractType1);
            submissionMetrics.PercentageContractType2 = Helpers.GetPercentage(submissionMetrics.ContractType2,
                submissionSummary.DcEarnings.ContractType2);
            submissionMetrics.Percentage = Helpers.GetPercentage(submissionMetrics.Total, submissionSummary.DcEarnings.Total);
            return submissionMetrics;
        }
    }
}