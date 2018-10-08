using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    // ReSharper disable once InconsistentNaming
    public interface IFM36LearnerToLearnerMapper
    {
        Learner Map(int ukprn, string collectionYear, FM36Learner learner);
    }
}