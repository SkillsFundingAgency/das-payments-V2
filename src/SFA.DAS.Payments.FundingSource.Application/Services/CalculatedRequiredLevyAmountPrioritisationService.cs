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

            orderedReturnList.AddRange(sourceList.WhereAndRemove(x => IsRefund(x)));


            orderedReturnList.AddRange(sourceList); //todo change this at the end

            return orderedReturnList;
        }

        

        private bool IsRefund(CalculatedRequiredLevyAmount calculatedRequiredLevyAmount)
        {
            return calculatedRequiredLevyAmount.AmountDue < 0;
        }
    }

    public static class CalculatedRequiredLevyAmountListExtensions
    {
        public static List<CalculatedRequiredLevyAmount> WhereAndRemove(this List<CalculatedRequiredLevyAmount> sourceList, Func<CalculatedRequiredLevyAmount, bool> action)
        {
            var matchedItems = sourceList.Where(action).ToList();
            matchedItems.ForEach(x => sourceList.Remove(x));
            return matchedItems;
        }
    }
}