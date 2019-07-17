using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators
{
    public class StandardLearnerRestarts :StandardLearner
    {
        public StandardLearnerRestarts(IEnumerable<Learner> learners, string featureNumber) : base(learners, featureNumber)
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, Learner request)
        {
            base.DoSpecificMutate(learner, request);
            DCT.TestDataGenerator.Helpers.AddLearningDeliveryRestartFAM(learner);
        }
    }
}
