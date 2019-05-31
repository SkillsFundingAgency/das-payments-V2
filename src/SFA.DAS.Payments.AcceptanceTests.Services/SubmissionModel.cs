using ESFA.DC.Jobs.Model.Enums;

namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    public class SubmissionModel
    {
        public SubmissionModel(JobType jobType, long ukprn)
        {
            JobType = jobType;
            Ukprn = ukprn;
        }

        public string CollectionName { get; set; }

        public int Period { get; set; }

        public string FileName { get; set; }

        public decimal FileSizeBytes { get; set; }

        public string SubmittedBy { get; set; }

        public long Ukprn { get; set; }

        public string NotifyEmail { get; set; }

        public JobType JobType { get; set; }

        public string StorageReference { get; set; }

        public int CollectionYear { get; set; }
    }
}
