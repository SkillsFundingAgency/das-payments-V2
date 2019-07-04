using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class Framework403Learner19PlusWithEHCPlanAndSmallEmployer : Framework403Learner
    {
        public Framework403Learner19PlusWithEHCPlanAndSmallEmployer(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            // need to override the DOB for this test.
            messageLearner.DateOfBirth = messageLearner.LearningDelivery.Single(ld => ld.AimType == 1).LearnStartDate.AddYears(-21);
        }
    }
}
