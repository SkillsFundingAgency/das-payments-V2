using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class SubmissionDataFactory
    {
        private SubmissionSummaryModel submissionSummaryModel;
        private CollectionPeriodToleranceModel collectionPeriodToleranceModel;

        private readonly SubmissionDataContext submissionDataContext;

        public SubmissionDataFactory(SubmissionDataContext submissionDataContext)
        {
            this.submissionDataContext = submissionDataContext;
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

        public SubmissionDataFactory SetPercentageWithInTolerance()
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

        public SubmissionDataFactory SetPercentageOutOfDefaultTolerance()
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

            submissionDataContext.Add(submissionSummaryModel);

            await submissionDataContext.SaveChangesAsync();
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

            submissionDataContext.Add(collectionPeriodToleranceModel);
            await submissionDataContext.SaveChangesAsync();
        }

        public async Task RemoveSubmissionSummaryModel(byte collectionPeriod, short academicYear)
        {
            var existingConfig = submissionDataContext.SubmissionSummaries.Where(c =>
                c.CollectionPeriod == collectionPeriod && c.AcademicYear == academicYear);

            submissionDataContext.SubmissionSummaries.RemoveRange(existingConfig);

            await submissionDataContext.SaveChangesAsync();
        }

        public async Task RemoveCollectionPeriodToleranceModel(byte collectionPeriod, short academicYear)
        {
            var existingConfig = submissionDataContext.CollectionPeriodTolerances.Where(c =>
                c.CollectionPeriod == collectionPeriod && c.AcademicYear == academicYear);

            submissionDataContext.CollectionPeriodTolerances.RemoveRange(existingConfig);

            await submissionDataContext.SaveChangesAsync();
        }

        public async Task RemoveSubmissionsSummaryModel(List<SubmissionsSummaryModel> data)
        {
            submissionDataContext.RemoveRange(data);
            await submissionDataContext.SaveChangesAsync();
        }

        public async Task ClearData(List<SubmissionsSummaryModel> data, byte collectionPeriod, short academicYear)
        {
            await RemoveSubmissionsSummaryModel(data);
            await RemovePeriodEndSubmissionWindowValidationJob(collectionPeriod, academicYear);
            await RemoveSubmissionSummaryModel(collectionPeriod, academicYear);
            await RemoveCollectionPeriodToleranceModel(collectionPeriod, academicYear);
        }

        private async Task RemovePeriodEndSubmissionWindowValidationJob(byte collectionPeriod, short academicYear)
        {
            var jobs = await submissionDataContext.Jobs
                .Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob &&
                            x.CollectionPeriod == collectionPeriod &&
                            x.AcademicYear == academicYear)
                .ToListAsync();

            submissionDataContext.RemoveRange(jobs);

            await submissionDataContext.SaveChangesAsync();
        }

        public async Task<JobModel> GetPeriodEndSubmissionWindowValidationJob(byte collectionPeriod, short academicYear)
        {
            return await submissionDataContext.Jobs
                .Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob &&
                            x.CollectionPeriod == collectionPeriod &&
                            x.AcademicYear == academicYear)
                .OrderByDescending(x => x.StartTime)
                .FirstOrDefaultAsync();
        }
    }
}
