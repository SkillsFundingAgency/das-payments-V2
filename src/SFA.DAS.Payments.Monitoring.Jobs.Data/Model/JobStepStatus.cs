namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public enum JobStepStatus : byte
    {
        Queued = 1, 
        Processing, 
        Completed, 
        Failed
    }
}