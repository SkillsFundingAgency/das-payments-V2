using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    public interface IApprenticeshipContractTypeEarningsEventBuilder
    {
        ApprenticeshipContractTypeEarningsEvent Build(long ukprn, string collectionYear, string jobId, FM36Learner fm36Learner);
    }
}