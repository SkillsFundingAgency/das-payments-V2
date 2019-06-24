using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework593Learner : FM36Base
    {
        public Framework593Learner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
           
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            var functionalSkillsLearningDelivery = messageLearner.LearningDelivery.First(ld => ld.AimType == 3);
            functionalSkillsLearningDelivery.LearnAimRef = learner.Aims.Count == 1
                ? "00300545"
                : learner.Aims.First(x => !x.IsMainAim).AimReference;
            functionalSkillsLearningDelivery.FundModel = 36;
            functionalSkillsLearningDelivery.ProgType = 20;
            functionalSkillsLearningDelivery.FworkCode = 593;
            functionalSkillsLearningDelivery.LearnStartDate =
                messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate;
        }

      
    }
}