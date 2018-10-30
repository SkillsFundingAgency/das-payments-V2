using System;
using System.Collections.Generic;
using System.Linq;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.AcceptanceTests.Core;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.EarningEvents.AcceptanceTests.Steps
{
    [Binding]
    public abstract class EarningsEventsStepsBase : StepsBase
    {
        protected List<LearnerEarnings> IlrLearnerEarnings { get => Get<List<LearnerEarnings>>(); set => Set(value); }

        protected EarningsEventsStepsBase(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

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
                        PriceEpisodeContractType = "ContractWithSfa",
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
                    var periodProperty = learningValues.GetType().GetProperty($"Property{period}");
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
            if (!string.IsNullOrEmpty(lastPeriod))
            {
                var periodProperty = learningValues.GetType().GetProperty($"Property{lastPeriod}");
                periodProperty?.SetValue(learningValues, learnerEarnings.CompletionAmount);
                priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);
            }

            var learningDelivery = learner.LearningDeliveries.FirstOrDefault(delivery => delivery.AimSeqNumber == 1); //TODO: will need to change to handle incentives
            if (learningDelivery == null)
            {
                learner.LearningDeliveries.Add(learningDelivery = new LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnAimRef = "ZPROD001",
                    }
                });
            }
        }
    }
}