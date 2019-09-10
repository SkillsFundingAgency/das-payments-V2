using System;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventFactory : IApprenticeshipContractTypeEarningsEventFactory
    {
        public ApprenticeshipContractTypeEarningsEvent Create(string contractType)
        {
            switch (contractType)
            {
                case ContractTypeValidationRule.Act1:
                case ContractTypeValidationRule.ContractForServicesWithEmployer:
                    return new ApprenticeshipContractType1EarningEvent();
                case ContractTypeValidationRule.Act2:
                case ContractTypeValidationRule.ContractForServicesWithSfa:
                    return new ApprenticeshipContractType2EarningEvent();
                default:
                    throw new InvalidOperationException($"Unknown contract type: '{contractType}'.");
            }
        }
    }
}