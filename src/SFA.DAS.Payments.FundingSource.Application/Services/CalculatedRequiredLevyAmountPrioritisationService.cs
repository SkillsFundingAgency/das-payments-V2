using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.FundingSource.Application.Extensions;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class CalculatedRequiredLevyAmountPrioritisationService : ICalculatedRequiredLevyAmountPrioritisationService
    {
        public List<CalculatedRequiredLevyAmount> Prioritise(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities)
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
}