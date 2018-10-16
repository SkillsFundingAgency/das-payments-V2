using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    public interface IApprenticeshipContractTypeEarningsEventBuilder
    {
        ApprenticeshipContractTypeEarningsEvent Build(IIlrLearnerSubmission learnerSubmission);
    }
}