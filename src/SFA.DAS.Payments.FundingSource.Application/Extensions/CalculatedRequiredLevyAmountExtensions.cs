using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Extensions
{
    public static class CalculatedRequiredLevyAmountExtensions
    {
        public static bool IsTransfer(this ITransferAccountIdsMessage message)
        {
            if (!message.AccountId.HasValue)
                throw new InvalidOperationException($"The account id of the levy message is invalid.");
            return message.TransferSenderAccountId.HasValue && 
                   message.TransferSenderAccountId != 0 && 
                   message.AccountId != message.TransferSenderAccountId;
        }

        public static List<CalculatedRequiredLevyAmount> GetMatchedItemsAndRemoveFromSource(this List<CalculatedRequiredLevyAmount> sourceList, Func<CalculatedRequiredLevyAmount, bool> action)
        {
            var matchedItems = sourceList.Where(action).ToList();
            matchedItems.ForEach(x => sourceList.Remove(x));
            return matchedItems;
        }

        public static bool IsRefund(this CalculatedRequiredLevyAmount calculatedRequiredLevyAmount)
        {
            return calculatedRequiredLevyAmount.AmountDue < 0;
        }

        public static List<CalculatedRequiredLevyAmount> PerformDefaultPaymentsSort(this List<CalculatedRequiredLevyAmount> calculatedRequiredLevyAmounts)
        {
            return calculatedRequiredLevyAmounts
                .OrderBy(x => x.AgreedOnDate)
                .ThenBy(x => x.Learner.Uln)
                .ToList();
        }

        public static long CalculateFundingAccountId(this ITransferAccountIdsMessage levyTransaction, bool isReceiverTransferPayment)
        {
            if (isReceiverTransferPayment)
                return levyTransaction.AccountId.GetValueOrDefault();

            return levyTransaction.IsTransfer() ? levyTransaction.TransferSenderAccountId.GetValueOrDefault() : levyTransaction.AccountId.GetValueOrDefault();
        }
    }
}