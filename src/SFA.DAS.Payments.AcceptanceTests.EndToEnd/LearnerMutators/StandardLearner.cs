using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class StandardLearner : Fm36Base
    {
        public StandardLearner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            var firstOnProgrammeDelivery = messageLearner.LearningDelivery.First(ld => ld.AimType == 1);
            var learningDelivery = messageLearner.LearningDelivery.Single(ld => ld.AimType == 3);
            learningDelivery.FundModel = 36;
            learningDelivery.ProgType = 25;
            learningDelivery.StdCode = firstOnProgrammeDelivery.StdCode;
            learningDelivery.StdCodeSpecified = true;
            learningDelivery.FworkCodeSpecified = false;
            learningDelivery.PwayCodeSpecified = false;
        }
    }
}
