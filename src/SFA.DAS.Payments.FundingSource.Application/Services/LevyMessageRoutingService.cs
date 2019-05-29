using System;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public interface ILevyMessageRoutingService
    {
        long GetDestinationAccountId(CalculatedRequiredLevyAmount message);
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
    }
}