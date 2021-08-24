using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2635-LearnStartDate-IsMapped-FromILR-ToPaymentsTable")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2635_Steps : FM36BreakInLearningBaseSteps
    {
        public PV2_2635_Steps(FeatureContext context) : base(context)
        {
        }

        //step not reusable
        [Then(@"the correct LearningStartDate is set for each generated payment")]
        public async Task ThenTheCorrectLearningStartDateIsSetForEachGeneratedPayment()
        {
            var payments = await DataContext.Payment
                .Where(x => x.LearnerReferenceNumber == TestSession.Learners.First().LearnRefNumber)
                .ToListAsync();

            payments.ForEach(p =>
            {
                DateTime expectedLearnStartDate;

                if (p.CollectionPeriod.Period == 4)
                {
                    expectedLearnStartDate = GetExpectedLearningStartDate(TestSession.FM36Global, p);
                }
                else
                {
                    expectedLearnStartDate = GetExpectedLearningStartDate(TestSession.PreviousFm36Global,p);
                }

                p.LearningStartDate.Should().Be(expectedLearnStartDate, $"Payment AimSeqNumber: {p.LearningAimSequenceNumber}");
            });
        }

        private DateTime GetExpectedLearningStartDate(FM36Global fm36, PaymentModel payment)
        {
            var learner = fm36.Learners.Single(l => l.LearnRefNumber == payment.LearnerReferenceNumber);
            var learningDelivery = learner.LearningDeliveries.Single(ld => ld.AimSeqNumber == payment.LearningAimSequenceNumber);

            return payment.TransactionType != TransactionType.OnProgrammeMathsAndEnglish
                ? learningDelivery.LearningDeliveryValues.LearnStartDate
                : learner.LearningDeliveries.Min(ld => ld.LearningDeliveryValues.LearnStartDate);
        }
    }
}