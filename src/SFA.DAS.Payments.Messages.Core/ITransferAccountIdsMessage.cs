
namespace SFA.DAS.Payments.Messages.Core
{
    public interface ITransferAccountIdsMessage
    {
        long? AccountId { get; set; }
        long? TransferSenderAccountId { get; set; }
    }
}
