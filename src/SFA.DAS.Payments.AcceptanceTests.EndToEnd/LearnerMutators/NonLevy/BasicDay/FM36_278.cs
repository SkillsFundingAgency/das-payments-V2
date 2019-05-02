using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class FM36_278 : SingleNonLevyLearner
    {
        public FM36_278(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "278")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            MutateHE(learner);
        }
    }
}
