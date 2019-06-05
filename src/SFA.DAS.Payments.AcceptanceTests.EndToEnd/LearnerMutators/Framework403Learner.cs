using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework403Learner : FM36Base
    {
        public Framework403Learner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            var functionalSkillsLearningDelivery = messageLearner.LearningDelivery.Single(ld => ld.AimType == 3);
            functionalSkillsLearningDelivery.LearnAimRef = "60005105";
            functionalSkillsLearningDelivery.FundModel = 36;
            functionalSkillsLearningDelivery.ProgType = 2;
            functionalSkillsLearningDelivery.FworkCode = 403;
            functionalSkillsLearningDelivery.LearnStartDate =
                messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate;
        }
    }
}