using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Application.UnitTests.Mapping.EarningEvent
{
    public class ApprenticeshipContractType2MappingTests : ApprenticeshipContractTypeMappingTests<ApprenticeshipContractType2EarningEvent>
    {
        public ApprenticeshipContractType2MappingTests() : base(ContractType.Act2)
        {
        }
    }
}