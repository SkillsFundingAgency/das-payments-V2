using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.SmallEmployer
{
    public class FM36_328 : Framework403Learner
    {
        public FM36_328(IEnumerable<Learner> learnerRequests) : base(learnerRequests, "328")
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            // need to override the DOB for this test.
            messageLearner.DateOfBirth = messageLearner.LearningDelivery[0].LearnStartDate.AddYears(-21);

            var firstLearnerEmploymentStatus = messageLearner.LearnerEmploymentStatus[0];
            var firstLearnerEmploymentStatusMonitoring = firstLearnerEmploymentStatus.EmploymentStatusMonitoring.ToList();
            firstLearnerEmploymentStatusMonitoring.Add(new MessageLearnerLearnerEmploymentStatusEmploymentStatusMonitoring()
            {
                ESMType = EmploymentStatusMonitoringType.SEM.ToString(),
                ESMCode = (int)EmploymentStatusMonitoringCode.SmallEmployer,
                ESMCodeSpecified = true
            });
            messageLearner.LearnerEmploymentStatus[0].EmploymentStatusMonitoring = firstLearnerEmploymentStatusMonitoring.ToArray();

            DCT.TestDataGenerator.Helpers.AddLearningDeliveryFAM(messageLearner, LearnDelFAMType.EEF, LearnDelFAMCode.EEF_Apprenticeship_19);

            messageLearner.LearningDelivery[0].AppFinRecord = messageLearner.LearningDelivery[0].AppFinRecord
                .Where(af => af.AFinType != LearnDelAppFinType.PMR.ToString()).ToArray();

            messageLearner.LearningDelivery[1].LearnAimRef = "60005105";
            messageLearner.LearningDelivery[1].LearnStartDate = messageLearner.LearningDelivery[0].LearnStartDate;

            MutateHigherEducation(messageLearner);
        }
    }
}
