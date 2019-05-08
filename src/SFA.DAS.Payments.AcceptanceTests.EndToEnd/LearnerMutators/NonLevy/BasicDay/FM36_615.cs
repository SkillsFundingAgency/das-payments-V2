using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class FM36_615 : SingleNonLevyLearner
    {
        public FM36_615(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests,"615")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest request)
        {
            learner.LearningDelivery[1].LearnAimRef = "6030571x";
        }
    }
}
