using SFA.DAS.Payments.PeriodEnd.Application.Repositories;
using SFA.DAS.Payments.PeriodEnd.Model.SubmissionJobs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.Application.Services
{
    public interface IProvidersRequiringReprocessingService
    {
        Task<SubmissionJobs> SuccessfulSubmissions(short academicYear, byte collectionPeriod);
    }

    public class ProvidersRequiringReprocessingService : IProvidersRequiringReprocessingService
    {
        private readonly IProvidersRequiringReprocessingRepository repository;
        
        public ProvidersRequiringReprocessingService(IProvidersRequiringReprocessingRepository providersRequiringResubmissionRepository)
        {
            this.repository = providersRequiringResubmissionRepository ??
                                                            throw new ArgumentNullException(nameof(providersRequiringResubmissionRepository));
        }

        public async Task<SubmissionJobs> SuccessfulSubmissions(short academicYear, byte collectionPeriod)
        {
            var providers = await repository.GetLatestSuccessfulJobs(academicYear, collectionPeriod);

            var providersToExclude = await repository.GetAll();

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