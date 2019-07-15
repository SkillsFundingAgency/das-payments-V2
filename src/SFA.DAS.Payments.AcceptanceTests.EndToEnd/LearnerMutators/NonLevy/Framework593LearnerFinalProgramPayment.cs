using System.Collections.Generic;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.ProviderChange
{
    public class Framework593LearnerFinalProgramPayment : Framework593Learner
    {
        public Framework593LearnerFinalProgramPayment(IEnumerable<Learner> learners) : base(learners, "609")
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            foreach (var messageLearnerLearningDelivery in messageLearner.LearningDelivery)
            {
                var learnPlanEndDate = messageLearnerLearningDelivery.LearnPlanEndDate.AddDays(-1);

                messageLearnerLearningDelivery.LearnPlanEndDate = learnPlanEndDate;
                messageLearnerLearningDelivery.LearnActEndDate = learnPlanEndDate;
                foreach (var deliveryFam in messageLearnerLearningDelivery.LearningDeliveryFAM)
                {
                    deliveryFam.LearnDelFAMDateTo = learnPlanEndDate;
                }
            }
        }
    }
}