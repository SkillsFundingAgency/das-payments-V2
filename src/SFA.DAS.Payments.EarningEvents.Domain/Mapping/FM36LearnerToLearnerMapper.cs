using System;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.EarningEvents.Domain.Mapping
{
    // ReSharper disable once InconsistentNaming
    public class FM36LearnerToLearnerMapper : IFM36LearnerToLearnerMapper
    {
        public Learner Map(int ukprn, string collectionYear, FM36Learner learner)
        {
            return new Learner
            {
                Ukprn = ukprn,
                ReferenceNumber = learner.LearnRefNumber,
                Uln = 0,
            };
        }
    }
}