using System;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventFactory : IApprenticeshipContractTypeEarningsEventFactory
    {
        public const string Act1 = "ContractWithEmployer";
        public const string Act2 = "ContractWithSfa";

        public ApprenticeshipContractTypeEarningsEvent Create(string contractType)
        {
            switch (contractType)
            {
                case Act1:
                    return new ApprenticeshipContractType1EarningEvent();
                case Act2:
                    return new ApprenticeshipContractType2EarningEvent();
                default:
                    throw new InvalidOperationException($"Unknown contract type: '{contractType}'.");
            }
        }
    }
}