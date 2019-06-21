using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class Framework593Learner : Fm36Base
    {
        public Framework593Learner(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
           
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            SetFrameworkComponentAimDetails(messageLearner,"00300545");
        }
    }
}