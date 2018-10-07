using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    public interface ILearnerToEarningsEventMapper
    {
        IEarningEvent Map(FM36Learner learner);
    }
}