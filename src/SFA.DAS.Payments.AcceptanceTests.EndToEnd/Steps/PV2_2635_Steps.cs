using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2635-LearnStartDate-IsMapped-FromILR-ToPaymentsTable")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2635_Steps : PV2_2268_Steps
    {
        public PV2_2635_Steps(FeatureContext context) : base(context)
        {
        }

        //step not reusable
        [Then(@"the correct LearningStartDate is set for each generated payment")]
        public async Task ThenTheCorrectLearningStartDateIsSetForEachGeneratedPayment()
        {
            var payments = await DataContext.Payment
                .Where(x => x.JobId == TestSession.JobId)
                .ToListAsync();

            payments.ForEach(p =>
            {
                DateTime expectedLearnStartDate;
                LearningDelivery learningDelivery;

                if (p.CollectionPeriod.Period == 4)
                {
                    var learner = TestSession.FM36Global.Learners.Single(l => l.LearnRefNumber == p.LearnerReferenceNumber);
                    learningDelivery = learner.LearningDeliveries.Single(ld => ld.AimSeqNumber == p.LearningAimSequenceNumber);
                    expectedLearnStartDate = learningDelivery.LearningDeliveryValues.LearnStartDate;
                }
                else
                {
                    var learner = TestSession.PreviousFm36Global.Learners.Single(l => l.LearnRefNumber == p.LearnerReferenceNumber);
                    learningDelivery = learner.LearningDeliveries.Single(ld => ld.AimSeqNumber == p.LearningAimSequenceNumber);
                    expectedLearnStartDate = learningDelivery.LearningDeliveryValues.LearnStartDate;
                }

                p.LearningStartDate.Should().Be(expectedLearnStartDate, $"Payment AimSeqNumber: {p.LearningAimSequenceNumber}, FM36 AimSeqNumber: {learningDelivery.AimSeqNumber}");
            });
        }
    }
}