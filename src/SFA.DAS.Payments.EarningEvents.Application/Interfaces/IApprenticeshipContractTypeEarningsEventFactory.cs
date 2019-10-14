using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IApprenticeshipContractTypeEarningsEventFactory
    {
        ApprenticeshipContractTypeEarningsEvent Create(string contractType);
    }
}