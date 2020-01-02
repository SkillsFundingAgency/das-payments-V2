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
        void AddDataLockedEarnings(DataLockTypeAmounts dataLockedAmounts);
        void AddHeldBackCompletionPayments(ContractTypeAmounts heldBackCompletionPaymentAmounts);
        void AddRequiredPayments(List<TransactionTypeAmounts> requiredPaymentAmounts);
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
        private DataLockTypeAmounts dataLocked;
        private ContractTypeAmounts heldBackCompletionPayments;
        private List<TransactionTypeAmounts> requiredPayments;
        public SubmissionSummary(long ukprn, long jobId, byte collectionPeriod, short academicYear)
        {
            Ukprn = ukprn;
            JobId = jobId;
            CollectionPeriod = collectionPeriod;
            AcademicYear = academicYear;
            dcEarnings = new List<TransactionTypeAmounts>();
            dasEarnings = new List<TransactionTypeAmounts>();
            dataLocked = new DataLockTypeAmounts();
            requiredPayments = new List<TransactionTypeAmounts>();
            heldBackCompletionPayments = new ContractTypeAmounts();
        }

        public virtual void AddEarnings(List<TransactionTypeAmounts> dcEarningTransactionTypeAmounts, List<TransactionTypeAmounts> dasEarningTransactionTypeAmounts)
        {
            dcEarnings.Clear();
            dcEarnings.AddRange(dcEarningTransactionTypeAmounts);
            dasEarnings.Clear();
            dasEarnings.AddRange(dasEarningTransactionTypeAmounts);
        }

        public virtual void AddDataLockedEarnings(DataLockTypeAmounts dataLockedAmounts)
        {
            dataLocked = dataLockedAmounts ?? throw new ArgumentNullException(nameof(dataLockedAmounts));
        }

        public virtual void AddHeldBackCompletionPayments(ContractTypeAmounts heldBackCompletionPaymentAmounts)
        {
            heldBackCompletionPayments = heldBackCompletionPaymentAmounts ?? throw new ArgumentNullException(nameof(heldBackCompletionPaymentAmounts));
        }

        public virtual void AddRequiredPayments(List<TransactionTypeAmounts> requiredPaymentAmounts)
        {
            requiredPayments = requiredPaymentAmounts ?? throw new ArgumentNullException(nameof(requiredPaymentAmounts));
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
                DataLockedEarnings = dataLocked.Total,
                DataLockedPaymentsMetrics = new List<DataLockedEarningsModel> { new DataLockedEarningsModel { Amounts = dataLocked } },
                HeldBackCompletionPayments = heldBackCompletionPayments,
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
            result.SubmissionMetrics = new ContractTypeAmountsVerbose();
            result.SubmissionMetrics.ContractType1 = result.RequiredPayments.ContractType1 + result.DataLockedEarnings + result.HeldBackCompletionPayments.ContractType1;
            result.SubmissionMetrics.ContractType2 = result.RequiredPayments.ContractType2 + result.HeldBackCompletionPayments.ContractType2;
            result.SubmissionMetrics = GetSubmissionMetrics(result);
            result.Percentage = result.SubmissionMetrics.Percentage;
            return result;
        }

        private ContractTypeAmountsVerbose GetDcEarnings()
        {
            var contractTypes = dcEarnings.GroupBy(earning => earning.ContractType)
                    .Select(g => new { ContractType = g.Key, Amount = g.Sum(x => x.Total) })
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
                    .Select(g => new { ContractType = g.Key, Amount = g.Sum(x => x.Total) })
                    .ToList()
                ;
            var result = new ContractTypeAmountsVerbose
            {
                ContractType1 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act1)?.Amount ?? 0,
                ContractType2 = contractTypes.FirstOrDefault(x => x.ContractType == ContractType.Act2)?.Amount ?? 0,
            };
            result.DifferenceContractType1 = result.ContractType1 - dcContractTpe1;
            result.DifferenceContractType2 = result.ContractType2 - dcContractTpe2;
            result.PercentageContractType1 = dcContractTpe1 != 0 ? (result.ContractType1 / dcContractTpe1) * 100 : 0;
            result.PercentageContractType2 = dcContractTpe2 != 0 ? (result.ContractType2 / dcContractTpe2) * 100 : 0;
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
            var submissionMetrics = new ContractTypeAmountsVerbose();
            submissionMetrics.ContractType1 = submissionSummary.RequiredPayments.ContractType1 + submissionSummary.DataLockedEarnings + submissionSummary.HeldBackCompletionPayments.ContractType1;
            submissionMetrics.ContractType2 = submissionSummary.RequiredPayments.ContractType2 + submissionSummary.HeldBackCompletionPayments.ContractType2;
            submissionMetrics.DifferenceContractType1 =
                submissionMetrics.ContractType1 - submissionSummary.DcEarnings.ContractType1;
            submissionMetrics.DifferenceContractType2 =
                submissionMetrics.ContractType2 - submissionSummary.DcEarnings.ContractType2;
            submissionMetrics.PercentageContractType1 = GetPercentage(submissionMetrics.ContractType1 , submissionSummary.DcEarnings.ContractType1) ;
            submissionMetrics.PercentageContractType2 = GetPercentage(submissionMetrics.ContractType2, submissionSummary.DcEarnings.ContractType2);
            return submissionMetrics;
        }

        private static decimal GetPercentage(decimal amount, decimal total) => total > 0 ? (amount / total) * 100 : 0;
    }
}