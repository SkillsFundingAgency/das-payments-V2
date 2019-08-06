namespace SFA.DAS.Payments.Messages.Core
{
    public interface IJobMessage: IPaymentsMessage
    {
        long JobId { get; }
    }
}