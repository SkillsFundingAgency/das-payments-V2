using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IRedundancyEarningEventFactory
    {
        ApprenticeshipContractTypeEarningsEvent CreateRedundancyContractType(
            ApprenticeshipContractTypeEarningsEvent earningEvent);

        FunctionalSkillEarningsEvent CreateRedundancyFunctionalSkillType(FunctionalSkillEarningsEvent functionalSkillEarning);
    }
}