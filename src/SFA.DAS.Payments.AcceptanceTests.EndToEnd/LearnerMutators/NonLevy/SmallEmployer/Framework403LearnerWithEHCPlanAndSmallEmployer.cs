using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class Framework403LearnerWithEHCPlanAndSmallEmployer : Framework403Learner
    {
        public Framework403LearnerWithEHCPlanAndSmallEmployer(IEnumerable<Learner> learners) : base(learners, "328")
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            // need to override the DOB for this test.
            messageLearner.DateOfBirth = messageLearner.LearningDelivery[0].LearnStartDate.AddYears(-21);

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(messageLearner, LearnDelFAMType.EEF, LearnDelFAMCode.EEF_Apprenticeship_19);

            RemovePmrRecord(messageLearner);

            messageLearner.LearningDelivery[1].LearnAimRef = "60005105";
        }
    }
}
