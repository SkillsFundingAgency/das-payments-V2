using System;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Extensions
{
    public static class CalculatedRequiredLevyAmountExtensions
    {
        public static bool IsTransfer(this CalculatedRequiredLevyAmount message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"The account id of the levy message is invalid.");
            return message.TransferSenderAccountId.HasValue && message.AccountId != message.TransferSenderAccountId;
        }
    }
}