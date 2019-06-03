using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.Model.Loose;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class Framework593LearnerWithdrawsAfterPlannedEndDate : Framework593Learner
    {
        public Framework593LearnerWithdrawsAfterPlannedEndDate(IEnumerable<Learner> learners) : base(learners, "278")
        {
        }

        protected override void DoSpecificMutate(MessageLearner messageLearner, Learner learner)
        {
            base.DoSpecificMutate(messageLearner, learner);

            foreach (var messageLearnerLearningDelivery in messageLearner.LearningDelivery)
            {
                var learnerLearningAim = learner.Aims.SingleOrDefault(l => l.AimReference == messageLearnerLearningDelivery.LearnAimRef);
                if (learnerLearningAim == null)
                {
                    continue;
                }

                SetDeliveryAsWithdrawn(messageLearnerLearningDelivery, messageLearner.LearningDelivery.First().LearnPlanEndDate.AddMonths(1), learnerLearningAim);

                SetupLearningDeliveryFam(messageLearnerLearningDelivery, learnerLearningAim);

                SetupAppFinRecord(messageLearner, messageLearnerLearningDelivery, learnerLearningAim);
            }

            messageLearner.LearnerEmploymentStatus[0].DateEmpStatApp = messageLearner.LearningDelivery[0].LearnStartDate.AddDays(-2);

            var functionalSkillsLearningDelivery = messageLearner.LearningDelivery.Single(learningDelivery => learningDelivery.AimType == 3);
            ProcessMessageLearnerForLearnerRequestOriginatingFromTrainingRecord(functionalSkillsLearningDelivery,
                learner.Aims.First());
        }

        private void SetDeliveryAsWithdrawn(MessageLearnerLearningDelivery delivery, DateTime actualEndDate, Aim learnerRequestAim)
        {
            base.SetDeliveryAsWithdrawn(delivery, learnerRequestAim);

            delivery.LearnActEndDate = actualEndDate;
            delivery.LearnActEndDateSpecified = true;
        }
    }
}
