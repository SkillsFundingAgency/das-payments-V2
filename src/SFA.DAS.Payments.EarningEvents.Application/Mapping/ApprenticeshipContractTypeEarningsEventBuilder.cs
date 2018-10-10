using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Mapping
{
    public class ApprenticeshipContractTypeEarningsEventBuilder: IApprenticeshipContractTypeEarningsEventBuilder
    {
        public ApprenticeshipContractTypeEarningsEvent Build(long ukprn, string collectionYear, string jobId, FM36Learner fm36Learner)
        {
            throw new NotImplementedException();
        }
    }
}