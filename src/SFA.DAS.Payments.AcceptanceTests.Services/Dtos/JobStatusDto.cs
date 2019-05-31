namespace SFA.DAS.Payments.AcceptanceTests.Services.Dtos
{
    using ESFA.DC.JobStatus.Interface;

    public class JobStatusDto
    {
        public JobStatusDto() { }

        public JobStatusDto(long jobId, JobStatusType jobStatus, int numberOfLearners = -1)
        {
            JobId = jobId;
            JobStatus = jobStatus;
            NumberOfLearners = numberOfLearners;
        }

        public long JobId { get; set; }

        public JobStatusType JobStatus { get; set; }

        public int NumberOfLearners { get; set; }
    }
}
