using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Model.SubmissionJobs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.Application.Services
{
    public interface ISubmissionJobsService
    {
        Task<SubmissionJobs> SuccessfulSubmissions();
    }

    public class SubmissionJobsService : ISubmissionJobsService
    {
        private readonly IPeriodEndRepository repository;
        private readonly IProvidersRequiringReprocessingRepository providersRequiringResubmissionRepository;
        
        public SubmissionJobsService(IPeriodEndRepository repository, IProvidersRequiringReprocessingRepository providersRequiringResubmissionRepository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.providersRequiringResubmissionRepository = providersRequiringResubmissionRepository ??
                                                            throw new ArgumentNullException(nameof(providersRequiringResubmissionRepository));
        }

        public async Task<SubmissionJobs> SuccessfulSubmissions()
        {
            var providers = await repository.GetLatestSuccessfulJobs();

            var providersToExclude = await providersRequiringResubmissionRepository.GetAll();

            providers.RemoveAll(x => providersToExclude.Contains(x.Ukprn));

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