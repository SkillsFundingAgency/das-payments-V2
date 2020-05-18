using System.Collections.Generic;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ICalculatedRequiredLevyAmountPrioritisationService
    {
        List<CalculatedRequiredLevyAmount> Prioritise(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities);
    }
}