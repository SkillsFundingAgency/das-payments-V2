using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventFactory : IApprenticeshipContractTypeEarningsEventFactory
    {
        public const string Act1 = "Levy Contract";
        public const string Act2 = "Non-Levy Contract";
        public const string ContractForServicesWithEmployer = "Contract for services with the employer";
        public const string ContractForServicesWithSfa = "Contract for services with the ESFA";

        public ApprenticeshipContractTypeEarningsEvent Create(string contractTypeAsString)
        {

            var contractType = MappingExtensions.GetContractType(contractTypeAsString);

            switch (contractType)
            {
                case ContractType.Act1:
                    return new ApprenticeshipContractType1EarningEvent();
                case ContractType.Act2:
                    return new ApprenticeshipContractType2EarningEvent();
                default:
                    return new ApprenticeshipContractTypeNoneEarningEvent();
            }
        }
    }
}