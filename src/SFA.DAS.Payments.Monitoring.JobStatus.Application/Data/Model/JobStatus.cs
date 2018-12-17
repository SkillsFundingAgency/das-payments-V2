namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model
{
    public enum JobStatus: byte
    {
        InProgress = 1,
        Completed, 
        CompletedWithErrors
    }
}