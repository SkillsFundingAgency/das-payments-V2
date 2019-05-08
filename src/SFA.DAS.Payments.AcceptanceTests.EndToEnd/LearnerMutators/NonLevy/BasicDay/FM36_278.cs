namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    using System.Collections.Generic;
    using System.Linq;
    using DCT.TestDataGenerator;
    using ESFA.DC.ILR.Model.Loose;

    public class FM36_278 : Framework593Over19Learner
    {
        public FM36_278(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "278")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            base.DoSpecificMutate(learner, learnerRequest);
            var ld = learner.LearningDelivery[0];

            foreach (var lds in learner.LearningDelivery)
            {
                lds.CompStatus = (int) CompStatus.Withdrawn;
                lds.CompStatusSpecified = true;

                lds.Outcome = (int)Outcome.NoAchievement;
                lds.OutcomeSpecified = true;

                lds.WithdrawReason = (int)WithDrawalReason.FinancialReasons;
                lds.WithdrawReasonSpecified = true;

                lds.LearnActEndDate = ld.LearnPlanEndDate.AddMonths(1);
                lds.LearnActEndDateSpecified = true;
            }

            var ldfam = ld.LearningDeliveryFAM.Single(ldf => ldf.LearnDelFAMType == LearnDelFAMType.ACT.ToString());

            ldfam.LearnDelFAMDateTo = ld.LearnActEndDate;
            ldfam.LearnDelFAMDateToSpecified = true;

            var appFinRecord =
                ld.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());

            if (appFinRecord == null)
            {
                Helpers.AddAfninRecord(learner, LearnDelAppFinType.TNP.ToString(), (int)LearnDelAppFinCode.TotalTrainingPrice, 15000);

                appFinRecord =
                    ld.AppFinRecord.SingleOrDefault(afr => afr.AFinType == LearnDelAppFinType.TNP.ToString());
            }

            appFinRecord.AFinDate = ld.LearnStartDate;
            appFinRecord.AFinDateSpecified = true;

            var lesm = learner.LearnerEmploymentStatus.ToList();
            lesm[0].DateEmpStatApp = learner.LearningDelivery[0].LearnStartDate.AddDays(-2);
        }
    }
}
