using System.Collections.Generic;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class FM36_334 : SingleNonLevyLearner
    {
        public FM36_334(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "334")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }
    }
}
