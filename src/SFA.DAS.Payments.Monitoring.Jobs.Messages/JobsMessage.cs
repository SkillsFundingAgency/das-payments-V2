namespace SFA.DAS.Payments.Monitoring.Jobs.Messages
{
    public abstract class JobsMessage: IJobMessage
    {
        public long JobId { get; set; }
    }
}