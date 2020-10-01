using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IRedundancyEarningEventFactory
    {
        ApprenticeshipContractTypeEarningsEvent CreateRedundancyContractTypeEarningsEvent(
            ApprenticeshipContractTypeEarningsEvent earningEvent);

        FunctionalSkillEarningsEvent CreateRedundancyFunctionalSkillTypeEarningsEvent(FunctionalSkillEarningsEvent functionalSkillEarning);
    }
}