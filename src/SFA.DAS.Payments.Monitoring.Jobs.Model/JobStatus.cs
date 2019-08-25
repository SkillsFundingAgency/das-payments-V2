namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public enum JobStatus: byte
    {
        InProgress = 1,
        Completed, 
        CompletedWithErrors
    }
}