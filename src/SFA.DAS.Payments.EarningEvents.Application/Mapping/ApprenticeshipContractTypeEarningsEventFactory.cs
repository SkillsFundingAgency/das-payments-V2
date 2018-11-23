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
        public const string Act1 = "Act1";
        public const string Act2 = "Act2";
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