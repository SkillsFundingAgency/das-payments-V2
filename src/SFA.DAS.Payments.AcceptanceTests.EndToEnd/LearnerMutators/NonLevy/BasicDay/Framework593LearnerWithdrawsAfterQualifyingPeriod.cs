using System.Collections.Generic;
using System.Linq;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class Framework593LearnerWithdrawsAfterQualifyingPeriod : Framework593Learner
    {
        public Framework593LearnerWithdrawsAfterQualifyingPeriod(IEnumerable<LearnerRequest> learnerRequests) : base(
            learnerRequests, "277")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            base.DoSpecificMutate(learner, learnerRequest);

            var learnerLearningDelivery = learner.LearningDelivery[0];

            SetWithdrawalDetails(learner, learnerRequest);

            SetLearningDeliveryFAMEndDate(learnerLearningDelivery);

            UpdateFinancialRecord(learner, learnerLearningDelivery);
        }

        private void SetLearningDeliveryFAMEndDate(MessageLearnerLearningDelivery learningDelivery)
        {
            var learningDeliveryFam = learningDelivery.LearningDeliveryFAM.Single(ldf =>
                ldf.LearnDelFAMType == LearnDelFAMType.ACT.ToString());

            learningDeliveryFam.LearnDelFAMDateTo = learningDelivery.LearnActEndDate;
            learningDeliveryFam.LearnDelFAMDateToSpecified = true;
        }

        private void SetWithdrawalDetails(MessageLearner learner, LearnerRequest learnerRequest)
        {
            foreach (var learningDelivery in learner.LearningDelivery)
            {
                learningDelivery.Outcome = (int) Outcome.NoAchievement;
                learningDelivery.OutcomeSpecified = true;
                learningDelivery.WithdrawReason = (int) WithDrawalReason.FinancialReasons;
                learningDelivery.WithdrawReasonSpecified = true;
                if (learnerRequest.StartDate.HasValue && learnerRequest.ActualDurationInMonths.HasValue)
                    learningDelivery.LearnActEndDate =
                        learnerRequest.StartDate.Value.AddMonths(learnerRequest.ActualDurationInMonths.Value);

                learningDelivery.LearnActEndDateSpecified = true;
            }
        }

        private void UpdateFinancialRecord(MessageLearner learner, MessageLearnerLearningDelivery learningDelivery)
        {
            var appFinRecord =
                learningDelivery.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());

            if (appFinRecord == null)
            {
                DCT.TestDataGenerator.Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(),
                    (int) LearnDelAppFinCode.TotalTrainingPrice, 15000);

                appFinRecord =
                    learningDelivery.AppFinRecord.SingleOrDefault(afr =>
                        afr.AFinType == LearnDelAppFinType.TNP.ToString());
            }

            appFinRecord.AFinDate = learningDelivery.LearnStartDate;
            appFinRecord.AFinDateSpecified = true;
        }
    }
}