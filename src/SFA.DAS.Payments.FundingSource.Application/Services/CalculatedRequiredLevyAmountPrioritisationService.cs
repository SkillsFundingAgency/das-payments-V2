using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class CalculatedRequiredLevyAmountPrioritisationService : ICalculatedRequiredLevyAmountPrioritisationService
    {
        public async Task<List<CalculatedRequiredLevyAmount>> Prioritise(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities)
        {
            var orderedReturnList = new List<CalculatedRequiredLevyAmount>();

            orderedReturnList.AddRange(sourceList.WhereAndRemove(x => x.IsRefund()));
            orderedReturnList.AddRange(sourceList.WhereAndRemove(x=>x.IsTransfer()).PerformDefaultPaymentsSort());
            orderedReturnList.AddRange(AddRequiredPayments(sourceList, providerPriorities));
            orderedReturnList.AddRange(sourceList.WhereAndRemove(x =>true).PerformDefaultPaymentsSort());

            return orderedReturnList;
        }

        private List<CalculatedRequiredLevyAmount> AddRequiredPayments(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities)
        {
            var sortedRequiredPayments = new List<CalculatedRequiredLevyAmount>();

            foreach (var providerPriority in providerPriorities.OrderBy(pp=>pp.Order))
            {
                var prioritisedPayments
                    = sourceList
                    .WhereAndRemove(x => x.Ukprn == providerPriority.Ukprn)
                    .ToList();
                sortedRequiredPayments.AddRange(prioritisedPayments.PerformDefaultPaymentsSort());
            }

            return sortedRequiredPayments;
        }
    }

    public static class CalculatedRequiredLevyAmountExtensions
    {
        public static List<CalculatedRequiredLevyAmount> WhereAndRemove(this List<CalculatedRequiredLevyAmount> sourceList, Func<CalculatedRequiredLevyAmount, bool> action)
        {
            var matchedItems = sourceList.Where(action).ToList();
            matchedItems.ForEach(x => sourceList.Remove(x));
            return matchedItems;
        }

        public static bool IsTransfer(this CalculatedRequiredLevyAmount calculatedRequiredLevyAmount)
        {

            return calculatedRequiredLevyAmount.TransferSenderAccountId.HasValue &&
                   calculatedRequiredLevyAmount.AccountId != calculatedRequiredLevyAmount.TransferSenderAccountId &&
                   calculatedRequiredLevyAmount.TransferSenderAccountId != 0;
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
    }
}