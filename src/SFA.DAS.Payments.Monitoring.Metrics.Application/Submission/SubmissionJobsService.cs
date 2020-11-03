using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.Submission
{
    public interface ISubmissionJobsService
    {
        Task<SubmissionJobs> SuccessfulSubmissionsForCollectionPeriod(short academicYear, byte collectionPeriod);
    }

    public class SubmissionJobsService : ISubmissionJobsService
    {
        private readonly ISubmissionJobsRepository repository;

        public SubmissionJobsService(ISubmissionJobsRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<SubmissionJobs> SuccessfulSubmissionsForCollectionPeriod(short academicYear, byte collectionPeriod)
        {
            var providers = await repository.GetLatestSuccessfulJobsForCollectionPeriod(academicYear, collectionPeriod);
            return new SubmissionJobs
            {
                SuccessfulSubmissionJobs = providers.Select(x => new SubmissionJob
                {
                    JobId = x.DcJobId,
                    Ukprn = x.Ukprn,
                }).ToList(),
            };
        }
    }
}
