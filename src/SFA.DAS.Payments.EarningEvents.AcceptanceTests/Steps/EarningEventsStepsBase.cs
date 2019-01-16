using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.EarningEvents.AcceptanceTests.Data;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public abstract class EarningEventsStepsBase : StepsBase
    {
        public List<PriceEpisode> PriceEpisodes { get => Get<List<PriceEpisode>>(); set => Set(value); }
        protected EarningEventsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        protected List<LearnerEarnings> IlrLearnerEarnings { get => Get<List<LearnerEarnings>>(); set => Set(value); }


        protected void AddLearnerEarnings(FM36Learner learner, LearnerEarnings learnerEarnings)
        {
            var priceEpisodes = learner.PriceEpisodes;
            var priceEpisode =
                priceEpisodes.FirstOrDefault(pe => pe.PriceEpisodeIdentifier == learnerEarnings.PriceEpisodeIdentifier);
            if (priceEpisode == null)
            {
                priceEpisodes.Add(priceEpisode = new PriceEpisode
                {
                    PriceEpisodeIdentifier = learnerEarnings.PriceEpisodeIdentifier,
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeAimSeqNumber = learnerEarnings.AimSequenceNumber,
                        EpisodeStartDate = learnerEarnings.EpisodeStartDate.ToDate(),
                        PriceEpisodeCompletionElement = learnerEarnings.CompletionAmount,
                        PriceEpisodeCompleted = learnerEarnings.CompletionStatus.Equals("completed", StringComparison.OrdinalIgnoreCase),
                        TNP1 = learnerEarnings.TotalTrainingPrice,
                        TNP2 = learnerEarnings.TotalAssessmentPrice,
                        PriceEpisodeInstalmentValue = learnerEarnings.InstallmentAmount,
                        PriceEpisodePlannedInstalments = learnerEarnings.NumberOfInstallments,
                        PriceEpisodeActualInstalments = learnerEarnings.NumberOfInstallments,
                        PriceEpisodeBalancePayment = learnerEarnings.BalancingPayment,
                        PriceEpisodeFundLineType = learnerEarnings.FundingLineType,
                        PriceEpisodeBalanceValue = learnerEarnings.BalancingPayment,
                        PriceEpisodeCompletionPayment = learnerEarnings.CompletionAmount,
                        PriceEpisodeContractType = "ContractWithSfa",  //TODO: Needs to work for ACT1 too
                        PriceEpisodeOnProgPayment = learnerEarnings.InstallmentAmount,
                        PriceEpisodePlannedEndDate = learnerEarnings.LearnerStartDate.ToDate().AddMonths(learnerEarnings.NumberOfInstallments),
                        PriceEpisodeSFAContribPct = SfaContributionPercentage,
                    },
                    PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
                });
            }

            var learningValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeOnProgPayment",
            };
            learnerEarnings
                .GetPeriodsList()
                .ForEach(period =>
                {
                    var periodProperty = learningValues.GetType().GetProperty($"Period{period}");
                    periodProperty?.SetValue(learningValues, learnerEarnings.InstallmentAmount);
                });
            priceEpisode.PriceEpisodePeriodisedValues.Add(learningValues);
            var completionEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeCompletionPayment",
            };
            var lastPeriod = learnerEarnings
                .GetPeriodsList()
                .LastOrDefault();
            if (string.IsNullOrEmpty(lastPeriod))
                Assert.Fail("No periods defined.");

            {
                var periodProperty = completionEarnings.GetType().GetProperty($"Period{lastPeriod}");
                periodProperty?.SetValue(completionEarnings, learnerEarnings.CompletionAmount);
                priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);
            }

            var balancingEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeBalancePayment",
            };

            if (!string.IsNullOrEmpty(lastPeriod))
            {
                var periodProperty = balancingEarnings.GetType().GetProperty($"Period{lastPeriod}");
                periodProperty?.SetValue(balancingEarnings, learnerEarnings.BalancingPayment);
                priceEpisode.PriceEpisodePeriodisedValues.Add(balancingEarnings);
            }

            learner.LearningDeliveries.Add(new LearningDelivery
            {
                AimSeqNumber = learnerEarnings.AimSequenceNumber,
                LearningDeliveryValues = new LearningDeliveryValues
                {
                    LearnAimRef = learnerEarnings.AimReference,
                    StdCode = string.IsNullOrEmpty(learnerEarnings.StandardCode) ? 0 : int.Parse(learnerEarnings.StandardCode),
                    ProgType = string.IsNullOrEmpty(learnerEarnings.ProgrammeType) ? 0 : int.Parse(learnerEarnings.ProgrammeType),
                    LearnDelInitialFundLineType = learnerEarnings.FundingLineType,
                }
            });
        }
    }
}