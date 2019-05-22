using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class FM36_328 : Framework403Learner
    {
        public FM36_328(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "328")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            // need to override the DOB for this test.
            learner.DateOfBirth = learner.LearningDelivery[0].LearnStartDate.AddYears(-21);

            var les1 = learner.LearnerEmploymentStatus[0];
            var lesm1 = les1.EmploymentStatusMonitoring.ToList();
            lesm1.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
            {
                ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                ESMCode = (int)EmploymentStatusMonitoringCode.SmallEmployer,
                ESMCodeSpecified = true
            });
            learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = lesm1.ToArray();

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(learner, LearnDelFAMType.EEF, LearnDelFAMCode.EEF_Apprenticeship_19);

            learner.LearningDelivery[0].AppFinRecord = learner.LearningDelivery[0].AppFinRecord
                .Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray();

            learner.LearningDelivery[1].LearnAimRef = "60005105";

            MutateHE(learner);
        }
    }
}
