namespace SFA.DAS.Payments.Monitoring.Jobs.Data.Model
{
    public enum JobMessageStatus : byte
    {
        Queued = 1, 
        Processing, 
        Completed, 
        Failed
    }
}