using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.JobStatus.Interface;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public interface IJobService
    {
        Task<JobStatusType> GetJobStatus(long jobId);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<long> SubmitJob(SubmissionModel submissionMessage);

        Task DeleteJob(long jobId);

        Task<IEnumerable<long>> GetJobsByStatus(int ukprn, params int[] status);
    }
}
