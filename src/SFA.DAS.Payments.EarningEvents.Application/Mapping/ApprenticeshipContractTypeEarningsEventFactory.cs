using System;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public interface IApprenticeshipContractTypeEarningsEventFactory
    {
        ApprenticeshipContractTypeEarningsEvent Create(string contractType);
    }

    public class ApprenticeshipContractTypeEarningsEventFactory: IApprenticeshipContractTypeEarningsEventFactory
    {
        public const string SfaContractType = "ContractWithSfa";
        public const string EmployerContractType = "ContractWithEmployer";
        public ApprenticeshipContractTypeEarningsEvent Create(string contractType)
        {
            switch (contractType)
            {
                case SfaContractType:
                    return new ApprenticeshipContractType1EarningEvent();
                case EmployerContractType:
                    return new ApprenticeshipContractType2EarningEvent();
                default: 
                    throw new InvalidOperationException($"Unknown contract type: '{contractType}'.");
            }
        }
    }
}