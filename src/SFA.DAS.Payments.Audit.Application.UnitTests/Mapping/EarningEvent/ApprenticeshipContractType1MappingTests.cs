using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public class ApprenticeshipContractType1MappingTests : ApprenticeshipContractTypeMappingTests<ApprenticeshipContractType1EarningEvent>
    {
        public ApprenticeshipContractType1MappingTests() : base(ContractType.Act1)
        {
        }
    }
}