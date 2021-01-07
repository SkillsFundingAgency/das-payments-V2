using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission
{
    public interface ISubmissionsSummary
    {
        SubmissionsSummaryModel GetMetrics(long jobId, short academicYear, byte collectionPeriod, IList<SubmissionSummaryModel> submissions);
        void CalculateIsWithinTolerance(decimal? lowerTolerance, decimal? upperTolerance);
    }

    public class SubmissionsSummary : ISubmissionsSummary
    {
        private SubmissionsSummaryModel submissionsSummaryModel;

        public SubmissionsSummaryModel GetMetrics(long jobId, short academicYear, byte collectionPeriod, IList<SubmissionSummaryModel> submissions)
        {
            if (submissions == null || submissions.Count == 0) return null;

            var submissionMetricsContractType1 = submissions.Sum(s => s.SubmissionMetrics.ContractType1);
            var submissionMetricsContractType2 = submissions.Sum(s => s.SubmissionMetrics.ContractType2);

            var dcEarningsContractType1 = submissions.Sum(s => s.DcEarnings.ContractType1);
            var dcEarningsContractType2 = submissions.Sum(s => s.DcEarnings.ContractType2);

            var dasEarningsContractType1 = submissions.Sum(s => s.DasEarnings.ContractType1);
            var dasEarningsContractType2 = submissions.Sum(s => s.DasEarnings.ContractType2);
            var percentage = Helpers.GetPercentage(submissionMetricsContractType1 + submissionMetricsContractType2, dcEarningsContractType1 + dcEarningsContractType2);

            submissionsSummaryModel = new SubmissionsSummaryModel
            {
                JobId = jobId,
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                Percentage = percentage,
                SubmissionMetrics = new ContractTypeAmountsVerbose
                {
                    ContractType1 = submissionMetricsContractType1,
                    ContractType2 = submissionMetricsContractType2,
                    DifferenceContractType1 = submissionMetricsContractType1 - dcEarningsContractType1,
                    DifferenceContractType2 = submissionMetricsContractType2 - dcEarningsContractType2,
                    PercentageContractType1 = Helpers.GetPercentage(submissionMetricsContractType1, dcEarningsContractType1),
                    PercentageContractType2 = Helpers.GetPercentage(submissionMetricsContractType2, dcEarningsContractType2),
                    Percentage = percentage,
                },
                DcEarnings = new ContractTypeAmounts
                {
                    ContractType1 = dcEarningsContractType1,
                    ContractType2 = dcEarningsContractType2,
                },
                DasEarnings = new ContractTypeAmountsVerbose
                {
                    ContractType1 = dasEarningsContractType1,
                    ContractType2 = dasEarningsContractType2,
                    DifferenceContractType1 = dasEarningsContractType1 - dcEarningsContractType1,
                    DifferenceContractType2 = dasEarningsContractType2 - dcEarningsContractType2,
                    PercentageContractType1 = Helpers.GetPercentage(dasEarningsContractType1, dcEarningsContractType1),
                    PercentageContractType2 = Helpers.GetPercentage(dasEarningsContractType2, dcEarningsContractType2),
                    Percentage = Helpers.GetPercentage(dasEarningsContractType1 + dasEarningsContractType2, dcEarningsContractType1 + dcEarningsContractType2),
                },
                AdjustedDataLockedEarnings = submissions.Sum(s => s.AdjustedDataLockedEarnings),
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
                },
                DataLockMetricsTotals = new DataLockCountsTotalsModel
                {
                    Amounts = new DataLockTypeCounts
                    {
                        DataLock1 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock1),
                        DataLock2 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock2),
                        DataLock3 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock3),
                        DataLock4 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock4),
                        DataLock5 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock5),
                        DataLock6 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock6),
                        DataLock7 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock7),
                        DataLock8 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock8),
                        DataLock9 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock9),
                        DataLock10 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock10),
                        DataLock11 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock11),
                        DataLock12 = submissions.Sum(s => s.DataLockMetrics.Amounts.DataLock12)
                    }
                }
            };

            return submissionsSummaryModel;
        }

        public void CalculateIsWithinTolerance(decimal? lowerTolerance, decimal? upperTolerance)
        {
            lowerTolerance = lowerTolerance ?? 99.92m;
            upperTolerance = upperTolerance ?? 100.08m;

            if (submissionsSummaryModel == null) return;

            submissionsSummaryModel.IsWithinTolerance = submissionsSummaryModel.Percentage >= lowerTolerance && submissionsSummaryModel.Percentage <= upperTolerance;
        }
    }
}