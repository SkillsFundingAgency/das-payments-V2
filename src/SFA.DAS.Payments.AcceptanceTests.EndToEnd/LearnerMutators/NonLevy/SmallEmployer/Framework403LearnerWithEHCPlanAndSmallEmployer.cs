using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class Framework403LearnerWithEHCPlanAndSmallEmployer : Framework403Learner
    {
        public Framework403LearnerWithEHCPlanAndSmallEmployer(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "328")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            // need to override the DOB for this test.
            learner.DateOfBirth = learner.LearningDelivery[0].LearnStartDate.AddYears(-21);

            UpdateEmploymentStatus(learner);

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(learner, LearnDelFAMType.EEF, LearnDelFAMCode.EEF_Apprenticeship_19);

            RemovePMRRecord(learner);

            learner.LearningDelivery[1].LearnAimRef = "60005105";

            MutateHigherEducation(learner);
        }

        private void RemovePMRRecord(MessageLearner learner)
        {
            learner.LearningDelivery[0].AppFinRecord = learner.LearningDelivery[0].AppFinRecord
                .Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray();
        }

        private void UpdateEmploymentStatus(MessageLearner learner)
        {
            var employmentStatusMonitoring = learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring.ToList();
            employmentStatusMonitoring.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
            {
                ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                ESMCode = (int) EmploymentStatusMonitoringCode.SmallEmployer,
                ESMCodeSpecified = true
            });

            learner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = employmentStatusMonitoring.ToArray();
        }
    }
}
