using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public interface IJobService
    {
        Task<ESFA.DC.JobStatus.Interface.JobStatusType> GetJobStatus(long jobId);

        Task<string> UpdateJobStatus(long jobId, ESFA.DC.JobStatus.Interface.JobStatusType status);

        Task<long> SubmitJob(SubmissionModel submissionMessage);

        Task DeleteJob(long jobId);

        Task<IEnumerable<long>> GetJobsByStatus(int ukprn, params int[] status);
    }
}
