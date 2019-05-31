using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class Framework593LearnerWithdrawsAfterPlannedEndDate : Framework593Learner
    {
        public Framework593LearnerWithdrawsAfterPlannedEndDate(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "278")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            base.DoSpecificMutate(learner, learnerRequest);
            var learningDelivery = learner.LearningDelivery[0];

            SetWithdrawalDetails(learner, learningDelivery);

            SetLearningDeliveryFAMEndDate(learningDelivery);

            UpdateFinancialRecord(learner, learningDelivery);

            UpdateEmploymentStatus(learner);
        }

        private void UpdateEmploymentStatus(MessageLearner learner)
        {
            var lesm = learner.LearnerEmploymentStatus.ToList();
            lesm[0].DateEmpStatApp = learner.LearningDelivery[0].LearnStartDate.AddDays(-2);
        }

        private void SetLearningDeliveryFAMEndDate(MessageLearnerLearningDelivery learningDelivery)
        {
            var ldfam = learningDelivery.LearningDeliveryFAM.Single(
                ldf => ldf.LearnDelFAMType == LearnDelFAMType.ACT.ToString());

            ldfam.LearnDelFAMDateTo = learningDelivery.LearnActEndDate;
            ldfam.LearnDelFAMDateToSpecified = true;
        }

        private void UpdateFinancialRecord(MessageLearner learner, MessageLearnerLearningDelivery learnerLearningDelivery)
        {
            var appFinRecord =
                learnerLearningDelivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());

            if (appFinRecord == null)
            {
                DCT.TestDataGenerator.Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(),
                    (int) LearnDelAppFinCode.TotalTrainingPrice, 15000);

                appFinRecord =
                    learnerLearningDelivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());
            }

            appFinRecord.AFinDate = learnerLearningDelivery.LearnStartDate;
            appFinRecord.AFinDateSpecified = true;
        }

        private void SetWithdrawalDetails(MessageLearner learner, MessageLearnerLearningDelivery learnerLearningDelivery)
        {
            foreach (var learningDelivery in learner.LearningDelivery)
            {
                learningDelivery.CompStatus = (int) CompStatus.Withdrawn;
                learningDelivery.CompStatusSpecified = true;

                learningDelivery.Outcome = (int) Outcome.NoAchievement;
                learningDelivery.OutcomeSpecified = true;

                learningDelivery.WithdrawReason = (int) WithDrawalReason.FinancialReasons;
                learningDelivery.WithdrawReasonSpecified = true;

                learningDelivery.LearnActEndDate = learnerLearningDelivery.LearnPlanEndDate.AddMonths(1);
                learningDelivery.LearnActEndDateSpecified = true;
            }
        }
    }
}
