using System;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventFactory : IApprenticeshipContractTypeEarningsEventFactory
    {
        public const string Act1 = "Levy Contract";
        public const string Act2 = "Non-Levy Contract";
        public const string ContractForServicesWithEmployer = "Contract for services with the employer";
        public const string ContractForServicesWithSfa = "Contract for services with the ESFA";

        public ApprenticeshipContractTypeEarningsEvent Create(string contractType)
        {
            switch (contractType)
            {
                case Act1:
                case ContractForServicesWithEmployer:
                    return new ApprenticeshipContractType1EarningEvent();
                case Act2:
                case ContractForServicesWithSfa:
                    return new ApprenticeshipContractType2EarningEvent();
                default:
                    throw new InvalidOperationException($"Unknown contract type: '{contractType}'.");
            }
        }
    }
}