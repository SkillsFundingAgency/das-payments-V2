using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class Framework403LearnerWithEHCPlanAndSmallEmployer : Framework403Learner
    {
        public Framework403LearnerWithEHCPlanAndSmallEmployer(IEnumerable<Learner> learners) : base(learners, "328")
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            // need to override the DOB for this test.
            messageLearner.DateOfBirth = messageLearner.LearningDelivery[0].LearnStartDate.AddYears(-21);

            UpdateEmploymentStatus(messageLearner);

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(messageLearner, LearnDelFAMType.EEF, LearnDelFAMCode.EEF_Apprenticeship_19);

            RemovePMRRecord(messageLearner);

            messageLearner.LearningDelivery[1].LearnAimRef = "60005105";

            MutateHigherEducation(messageLearner);
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
