namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public enum JobStatus: byte
    {
        InProgress = 1,
        Completed, 
        CompletedWithErrors
    }
}