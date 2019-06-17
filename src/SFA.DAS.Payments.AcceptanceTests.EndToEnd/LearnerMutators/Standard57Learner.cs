using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Standard57Learner : FM36Base
    {
        public Standard57Learner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            var learningDelivery = messageLearner.LearningDelivery.Single(ld => ld.AimType == 3);
            learningDelivery.LearnAimRef = "00300545";
            learningDelivery.FundModel = 36;
            learningDelivery.ProgType = 25;
            learningDelivery.StdCode = 57;
            learningDelivery.StdCodeSpecified = true;
            learningDelivery.FworkCodeSpecified = false;
            learningDelivery.PwayCodeSpecified = false;
            learningDelivery.LearnStartDate =
                messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate;
        }
    }
}
