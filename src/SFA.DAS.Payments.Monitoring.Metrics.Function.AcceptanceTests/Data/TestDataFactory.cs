using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Data
{
    public class TestDataFactory
    {
        private SubmissionSummaryModel submissionSummaryModel;
        private CollectionPeriodToleranceModel collectionPeriodToleranceModel;

        private readonly MetricsTestDataContext metricsTestDataContext;

        public TestDataFactory(MetricsTestDataContext metricsTestDataContext)
        {
            this.metricsTestDataContext = metricsTestDataContext;
        }

        public void CreateSubmissionSummaryModel(byte collectionPeriod, short academicYear)
        {
            submissionSummaryModel = new SubmissionSummaryModel
            {
                Percentage = 0,
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                JobId = 123,
                TotalDataLockedEarnings = 123,
                AlreadyPaidDataLockedEarnings = 123,
                AdjustedDataLockedEarnings = 123,
                Ukprn = 123,

                SubmissionMetrics = new ContractTypeAmountsVerbose(),
                DcEarnings = new ContractTypeAmounts(),
                DasEarnings = new ContractTypeAmountsVerbose(),
                HeldBackCompletionPayments = new ContractTypeAmounts(),
                RequiredPayments = new ContractTypeAmounts(),
                YearToDatePayments = new ContractTypeAmounts(),
            };
        }

        public TestDataFactory SetPercentageWithInTolerance()
        {
            submissionSummaryModel.DcEarnings.ContractType1 = 100;

            if (collectionPeriodToleranceModel != null)
            {
                var minvalue = Math.Min(collectionPeriodToleranceModel.SubmissionToleranceLower, collectionPeriodToleranceModel.SubmissionToleranceUpper);
                
                submissionSummaryModel.SubmissionMetrics.ContractType1 = minvalue;
            }
            else
            {
                submissionSummaryModel.SubmissionMetrics.ContractType1 = 100;
            }
            return this;
        }

        public TestDataFactory SetPercentageOutOfDefaultTolerance()
        {
            submissionSummaryModel.DcEarnings.ContractType1 = 100;

            if (collectionPeriodToleranceModel != null)
            {
                var minvalue = Math.Min(collectionPeriodToleranceModel.SubmissionToleranceLower, collectionPeriodToleranceModel.SubmissionToleranceUpper);

                submissionSummaryModel.SubmissionMetrics.ContractType1 = minvalue - 0.01m;
            }
            else
            {
                submissionSummaryModel.SubmissionMetrics.ContractType1 = 90.00m;
            }

            return this;
        }

        public async Task SaveModel()
        {
            if (submissionSummaryModel == null) return;

            await RemoveSubmissionSummaryModel(submissionSummaryModel.CollectionPeriod, submissionSummaryModel.AcademicYear);

            metricsTestDataContext.Add(submissionSummaryModel);

            await metricsTestDataContext.SaveChangesAsync();
        }

        public async Task CreateCollectionPeriodToleranceModel(byte collectionPeriod, short academicYear, decimal lowerTolerance, decimal upperTolerance)
        {
            collectionPeriodToleranceModel = new CollectionPeriodToleranceModel
            {
                CollectionPeriod = collectionPeriod,
                AcademicYear = academicYear,
                SubmissionToleranceLower = lowerTolerance,
                SubmissionToleranceUpper = upperTolerance
            };

            await RemoveCollectionPeriodToleranceModel(collectionPeriod, academicYear);

            metricsTestDataContext.Add(collectionPeriodToleranceModel);
            await metricsTestDataContext.SaveChangesAsync();
        }

        public async Task RemoveSubmissionSummaryModel(byte collectionPeriod, short academicYear)
        {
            var existingConfig = metricsTestDataContext.SubmissionSummaries.Where(c =>
                c.CollectionPeriod == collectionPeriod && c.AcademicYear == academicYear);

            metricsTestDataContext.SubmissionSummaries.RemoveRange(existingConfig);

            await metricsTestDataContext.SaveChangesAsync();
        }

        public async Task RemoveCollectionPeriodToleranceModel(byte collectionPeriod, short academicYear)
        {
            var existingConfig = metricsTestDataContext.CollectionPeriodTolerances.Where(c =>
                c.CollectionPeriod == collectionPeriod && c.AcademicYear == academicYear);

            metricsTestDataContext.CollectionPeriodTolerances.RemoveRange(existingConfig);

            await metricsTestDataContext.SaveChangesAsync();
        }

        public async Task RemoveSubmissionsSummaryModel(List<SubmissionsSummaryModel> data)
        {
            metricsTestDataContext.RemoveRange(data);
            await metricsTestDataContext.SaveChangesAsync();
        }

        public async Task ClearData(List<SubmissionsSummaryModel> data, byte collectionPeriod, short academicYear)
        {
            await RemoveSubmissionsSummaryModel(data);
            await RemoveSubmissionSummaryModel(collectionPeriod, academicYear);
            await RemoveCollectionPeriodToleranceModel(collectionPeriod, academicYear);
        }

        public async Task<List<SubmissionsSummaryModel>> GetSubmissionsSummaries(byte collectionPeriod, short academicYear)
        {
           return await metricsTestDataContext.SubmissionsSummaries.Where(s =>
                s.CollectionPeriod == collectionPeriod && s.AcademicYear == academicYear).ToListAsync();
        }
    }
}
