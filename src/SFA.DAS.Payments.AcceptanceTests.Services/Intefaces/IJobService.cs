using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Jobs.Model;
using ESFA.DC.Jobs.Model.Enums;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public interface IJobService
    {
        Task<JobStatusType> GetJobStatus(long jobId);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<long> SubmitJob(SubmissionModel submissionMessage);

        Task DeleteJob(long jobId);

        Task<bool> IsProviderInAnActiveJob(int ukprn);

        Task<FileUploadJob> GetJob(long ukprn, long jobId);
    }
}
