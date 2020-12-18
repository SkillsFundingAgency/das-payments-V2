using Newtonsoft.Json;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.Submission
{
    public class DataLockCountsTotalsModel
    {
        public long Id { get; set; }
        public long SubmissionsSummaryId { get; set; }
        
        [JsonIgnore]
        public virtual SubmissionsSummaryModel SubmissionsSummary { get; set; }
        public DataLockTypeCounts Amounts { get; set; } = new DataLockTypeCounts();
    }
}