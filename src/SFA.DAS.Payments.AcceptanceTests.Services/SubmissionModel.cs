using ESFA.DC.Jobs.Model.Enums;

namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    public class SubmissionModel
    {
        public SubmissionModel(ESFA.DC.Jobs.Model.Enums.EnumJobType jobType, long ukprn)
        {
            JobType = jobType;
            Ukprn = ukprn;
        }

        public string CollectionName { get; set; }

        public int Period { get; set; }

        public string FileName { get; set; }

        public decimal FileSizeBytes { get; set; }

        public string CreatedBy { get; set; }

        public long Ukprn { get; set; }

        public string NotifyEmail { get; set; }

        public ESFA.DC.Jobs.Model.Enums.EnumJobType JobType { get; set; }

        public string StorageReference { get; set; }

        public int CollectionYear { get; set; }
    }
}
