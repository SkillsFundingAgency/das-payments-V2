using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    public class EndToEndSteps : StepsBase
    {
        private readonly DcHelper dcHelper;
        private List<Training> ILR;

        public EndToEndSteps(FeatureContext context) : base(context)
        {
            dcHelper = new DcHelper(Container);
        }

        [Given(@"the provider is providing trainging for the following learners")]
        public void GivenTheProviderIsProvidingTraingingForTheFollowingLearners(Table table)
        {
            ILR = table.CreateSet<Training>().ToList();
            SfaContributionPercentage = ILR[0].SfaContributionPercentage;
        }


        [When(@"the ILR file is submitted for the learners for collection period R(.*)/Current Academic Year")]
        public async Task WhenTheILRFileIsSubmittedForTheLearnersForCollectionPeriodRCurrentAcademicYear(int period)
        {
            SetCurrentCollectionYear();
            CollectionPeriod = (byte) period;
            var fm36Learners = new List<FM36Learner>();
            foreach (var training in ILR)
            {
                var learner = new FM36Learner();
                PopulateLearner(learner, training);
                fm36Learners.Add(learner);
            }

            await dcHelper.SendIlrSubmission(fm36Learners, TestSession.Ukprn, CollectionYear);
        }

        private void PopulateLearner(FM36Learner learner, Training learnerEarnings)
        {
            var priceEpisode = new PriceEpisode
            {
                PriceEpisodeIdentifier = learnerEarnings.LearnerId,
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = learnerEarnings.StartDate.ToDate(),
                    PriceEpisodeCompletionElement = learnerEarnings.CompletionAmount,
                    PriceEpisodeCompleted = learnerEarnings.CompletionStatus.Equals("completed", StringComparison.OrdinalIgnoreCase),
                    TNP1 = learnerEarnings.TotalTrainingPrice,
                    TNP2 = learnerEarnings.TotalAssessmentPrice,
                    PriceEpisodeInstalmentValue = learnerEarnings.InstallmentAmount,
                    PriceEpisodePlannedInstalments = learnerEarnings.NumberOfInstallments,
                    PriceEpisodeActualInstalments = learnerEarnings.ActualInstallments,
                    PriceEpisodeBalancePayment = learnerEarnings.BalancingPayment,
                    //PriceEpisodeFundLineType = learnerEarnings.FundingLineType,
                    PriceEpisodeBalanceValue = learnerEarnings.BalancingPayment,
                    PriceEpisodeCompletionPayment = learnerEarnings.CompletionAmount,
                    PriceEpisodeContractType = "ContractWithEmployer", //TODO: Needs to work for ACT1 too
                    PriceEpisodeOnProgPayment = learnerEarnings.InstallmentAmount,
                    PriceEpisodePlannedEndDate = learnerEarnings.StartDate.ToDate().AddMonths(learnerEarnings.NumberOfInstallments),
                    PriceEpisodeSFAContribPct = SfaContributionPercentage,
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
            };

            var firstPeriodNumber = (int) (learnerEarnings.StartDate.ToDate().Subtract("start of academic year".ToDate()).TotalDays / 30) + 1;
            var lastPeriodNumber = firstPeriodNumber + learnerEarnings.NumberOfInstallments - 1;
            
            var learningValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeOnProgPayment",
            };

            PropertyInfo periodProperty;
            for (var i = firstPeriodNumber; i <= lastPeriodNumber; i++)
            {
                periodProperty = learningValues.GetType().GetProperty("Period" + i);
                periodProperty?.SetValue(learningValues, learnerEarnings.InstallmentAmount);
            }

            priceEpisode.PriceEpisodePeriodisedValues.Add(learningValues);

            learner.PriceEpisodes = new List<PriceEpisode>(new[] {priceEpisode});


            var completionEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeCompletionPayment",
            };

            periodProperty = completionEarnings.GetType().GetProperty("Period" + lastPeriodNumber);
            periodProperty?.SetValue(completionEarnings, learnerEarnings.CompletionAmount);
            priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);

            var balancingEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeBalancePayment",
            };

            periodProperty = balancingEarnings.GetType().GetProperty("Period" + lastPeriodNumber);
            periodProperty?.SetValue(balancingEarnings, learnerEarnings.BalancingPayment);
            priceEpisode.PriceEpisodePeriodisedValues.Add(balancingEarnings);

            learner.LearningDeliveries = new List<LearningDelivery>(new[]
            {
                new LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = "ZPROG001",
                    }
                }
            });
        }

        [Then(@"the following learner earnings should be generated")]
        public void ThenTheFollowingLearnerEarningsShouldBeGenerated(Table table)
        {
            var expectedEarnings = table.CreateSet<OnProgrammeEarning>().ToList();
            WaitForIt(() => EarningEventMatcher.MatchEarnings(expectedEarnings, TestSession.Ukprn), "OnProgrammeEarning event check failure");
        }

        [Then(@"the following payments will be calculated")]
        public void ThenTheFollowingPaymentsWillBeCalculated(Table table)
        {
            var expectedPayments = table.CreateSet<Payment>().ToList();
            WaitForIt(() => PaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "Payment event check failure");
        }

        [Then(@"the following provider payments will be generated")]
        public void ThenTheFollowingProviderPaymentsWillBeGenerated(Table table)
        {
            var expectedPayments = table.CreateSet<ProviderPayment>().ToList();
            WaitForIt(() => ProviderPaymentEventMatcher.MatchPayments(expectedPayments, TestSession.Ukprn), "ProviderPayment event check failure");
        }
    }
}
