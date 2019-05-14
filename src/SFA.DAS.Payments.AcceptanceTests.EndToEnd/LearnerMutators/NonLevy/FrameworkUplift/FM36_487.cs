namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.FrameworkUplift
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DCT.TestDataGenerator;
    using ESFA.DC.ILR.Model.Loose;

    public class FM36_487 : Framework403Learner
    {
        public FM36_487(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "487")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
        }
    }
}
