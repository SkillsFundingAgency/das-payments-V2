namespace SFA.DAS.Payments.Monitoring.JobStatus.Application.Data.Model
{
    public enum JobEventStatus : byte
    {
        Queued = 1, 
        Processing, 
        Completed, 
        Failed
    }
}