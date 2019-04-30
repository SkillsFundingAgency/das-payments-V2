using System.Collections.Generic;
using DCT.TestDataGenerator;
using ESFA.DC.ILR.Model.Loose;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators.NonLevy.BasicDay
{
    public class FM36_334 : SingleNonLevyLearner
    {
        public FM36_334(IEnumerable<LearnerRequest> learnerRequests) : base(learnerRequests, "334")
        {
        }

        protected override void DoSpecificMutate(MessageLearner learner, LearnerRequest learnerRequest)
        {
            if (learnerRequest.CompletionStatus == CompStatus.Completed)
            {
                // change AppFin to PMR
                var appfin = new List<MessageLearnerLearningDeliveryAppFinRecord>();
                appfin.Add(new MessageLearnerLearningDeliveryAppFinRecord()
                {
                    AFinAmount = learnerRequest.TotalTrainingPrice.Value,
                    AFinAmountSpecified = true,
                    AFinType = LearnDelAppFinType.PMR.ToString(),
                    AFinCode = (int)LearnDelAppFinCode.TrainingPayment,
                    AFinCodeSpecified = true,
                    AFinDate = learner.LearningDelivery[0].LearnActEndDate.AddMonths(-1),
                    AFinDateSpecified = true
                });

                learner.LearningDelivery[0].AppFinRecord = appfin.ToArray();
            }

            learner.LearningDelivery[1].LearnAimRef = "00300545";
            MutateHE(learner);
        }

    }
}
