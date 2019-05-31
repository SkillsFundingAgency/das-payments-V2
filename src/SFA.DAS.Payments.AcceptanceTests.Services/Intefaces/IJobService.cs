namespace SFA.DAS.Payments.AcceptanceTests.Services.Intefaces
{
    using System.Threading.Tasks;
    using ESFA.DC.JobStatus.Interface;

    public interface IJobService
    {
        Task<JobStatusType> GetJobStatus(long jobId);

        Task<string> UpdateJobStatus(long jobId, JobStatusType status);

        Task<long> SubmitJob(SubmissionModel submissionMessage);
    }
}
