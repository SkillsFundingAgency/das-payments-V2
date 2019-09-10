using System;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyMessageRoutingService
    {
        long GetDestinationAccountId(CalculatedRequiredLevyAmount message);
        bool IsTransfer(long accountId, long? transferSenderAccountId);
        long GetDestinationAccountId(long accountId, long? transferSenderAccountId);
    }

    public class LevyMessageRoutingService: ILevyMessageRoutingService
    {
        public long GetDestinationAccountId(CalculatedRequiredLevyAmount message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"The account id of the levy message is invalid.");
            return message.IsTransfer()
                ? message.TransferSenderAccountId.Value
                : message.AccountId.Value;
        }

        public bool IsTransfer(long accountId, long? transferSenderAccountId) =>
            transferSenderAccountId.HasValue &&
            transferSenderAccountId != 0 &&
            accountId != transferSenderAccountId;

        public long GetDestinationAccountId(long accountId, long? transferSenderAccountId)
        {
            return IsTransfer(accountId, transferSenderAccountId)
                ? transferSenderAccountId.Value
                : accountId;

        }
    }
}