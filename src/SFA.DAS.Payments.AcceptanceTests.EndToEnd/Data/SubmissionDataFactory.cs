using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class SubmissionDataFactory
    {
        private SubmissionSummaryModel submissionSummaryModel;

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

            submissionSummaryModel.SubmissionMetrics.ContractType1 = 100;

            return this;
        }

        public SubmissionDataFactory SetPercentageOutOfDefaultTolerance()
        {
            submissionSummaryModel.DcEarnings.ContractType1 = 100;

            submissionSummaryModel.SubmissionMetrics.ContractType1 = 90.00m;

            return this;
        }

        public async Task SaveModel()
        {
            if (submissionSummaryModel == null) return;

            submissionDataContext.Add(submissionSummaryModel);

            await submissionDataContext.SaveChangesAsync();
        }

        public async Task ClearData()
        {
            await RemovePeriodEndSubmissionWindowValidationJob();

            submissionDataContext.RemoveRange(submissionDataContext.SubmissionSummaries);
            submissionDataContext.RemoveRange(submissionDataContext.CollectionPeriodTolerances);
            submissionDataContext.RemoveRange(submissionDataContext.SubmissionsSummaries);
            
            await submissionDataContext.SaveChangesAsync();
        }

        private async Task RemovePeriodEndSubmissionWindowValidationJob()
        {
            var jobs = await submissionDataContext.Jobs
                .Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob)
                .ToListAsync();

            submissionDataContext.RemoveRange(jobs);

            await submissionDataContext.SaveChangesAsync();
        }

        public async Task<JobModel> GetPeriodEndSubmissionWindowValidationJob(long dcJobId, byte collectionPeriod, short academicYear)
        {
            return await submissionDataContext.Jobs
                .Where(x =>
                    x.DcJobId == dcJobId &&
                    x.JobType == JobType.PeriodEndSubmissionWindowValidationJob &&
                    x.CollectionPeriod == collectionPeriod &&
                    x.AcademicYear == academicYear)
                .OrderByDescending(x => x.StartTime)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}
