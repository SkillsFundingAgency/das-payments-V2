using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.Levy.BasicDay
{
    public class FM36_261 : SingleLevyLearner
    {
        public FM36_261(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "261")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest request)
        {
            learner.LearningDelivery[1].LearnAimRef = "60005105";
        }
    }
}
