using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    using System.Collections.Generic;
    using DCT.TestDataGenerator;
    using ESFA.DC.ILR.Model.Loose;

    public class FM36_277 : SingleNonLevyLearner
    {
        public FM36_277(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "277")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            var ld = learner.LearningDelivery[0];

            foreach (var lds in learner.LearningDelivery)
            {
                lds.Outcome = (int) Outcome.NoAchievement;
                lds.OutcomeSpecified = true;
                lds.WithdrawReason = (int) WithDrawalReason.FinancialReasons;
                lds.WithdrawReasonSpecified = true;
                if (learnerRequest.StartDate.HasValue && learnerRequest.ActualDurationInMonths.HasValue)
                {
                    lds.LearnActEndDate =
                        learnerRequest.StartDate.Value.AddMonths(learnerRequest.ActualDurationInMonths.Value);
                }

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


            learner.LearningDelivery[1].LearnAimRef = "6030571x";
        }
    }
}
