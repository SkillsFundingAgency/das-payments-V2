using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ICalculatedRequiredLevyAmountPrioritisationService
    {
        Task<List<CalculatedRequiredLevyAmount>> Prioritise(List<CalculatedRequiredLevyAmount> sourceList, List<(long Ukprn, int Order)> providerPriorities);
    }
}