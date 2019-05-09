using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class FM36_429 : SingleNonLevyLearner
    {
        public FM36_429(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "429")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            var ld = learner.LearningDelivery[0];
            var ldFams = ld.LearningDeliveryFAM.ToList();
            ldFams.Add(new MessageLearnerLearningDeliveryLearningDeliveryFAM()
            {
                LearnDelFAMType = LearnDelFAMType.LSF.ToString(),
                LearnDelFAMCode = ((int)LearnDelFAMCode.LSF).ToString(),
                LearnDelFAMDateFrom = ld.LearnStartDate,
                LearnDelFAMDateFromSpecified = true,
                LearnDelFAMDateTo = ld.LearnActEndDate,
                LearnDelFAMDateToSpecified = true,
            });
            ld.LearningDeliveryFAM = ldFams.ToArray();

            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }
    }
}
