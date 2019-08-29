namespace SFA.DAS.Payments.Monitoring.Jobs.Model
{
    public enum JobStepStatus : byte
    {
        Queued = 1, 
        Processing, 
        Completed, 
        Failed
    }
}