namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public class InProgressJobAverageJobCompletionTime
    {
        public long JobId { get; set; }
        public long? Ukprn { get; set; }
        public double? AverageJobCompletionTime { get; set; }
    }
}