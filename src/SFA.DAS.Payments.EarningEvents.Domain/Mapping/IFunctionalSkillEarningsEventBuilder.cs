using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    public interface IFunctionalSkillEarningsEventBuilder
    {
        FunctionalSkillEarningsEvent Build(ProcessLearnerCommand learnerSubmission);
    }
}