using System.Collections.Generic;
using System.Threading.Tasks;
using Enums = ESFA.DC.Jobs.Model.Enums;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    public interface IJobService
    {
        Task<Enums.JobStatusType> GetJobStatus(long jobId);

        Task<string> UpdateJobStatus(long jobId, Enums.JobStatusType status);

        Task<long> SubmitJob(SubmissionModel submissionMessage);

        Task DeleteJob(long jobId);

        Task<IEnumerable<long>> GetJobsByStatus(int ukprn, params int[] status);
    }
}
