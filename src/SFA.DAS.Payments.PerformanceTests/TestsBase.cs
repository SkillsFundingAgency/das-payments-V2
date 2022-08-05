using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Learner = SFA.DAS.Payments.AcceptanceTests.Core.Data.Learner;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.PerformanceTests
{
    public class TestsBase
    {
        public static TestsConfiguration Config => new TestsConfiguration();

        protected FM36Learner CreateFM36Learner(TestSession session, Learner testLearner, DateTime startDate)
        {
            var learner = new FM36Learner { LearnRefNumber = testLearner.LearnRefNumber, ULN = testLearner.Uln };
            var priceEpisode = new ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode
            {
                PriceEpisodeIdentifier = "pe-1",
                PriceEpisodeValues = new PriceEpisodeValues
                {
                    EpisodeStartDate = startDate,
                    EpisodeEffectiveTNPStartDate = startDate,
                    PriceEpisodeCompletionElement = 3000,
                    PriceEpisodeCompleted = false,
                    PriceEpisodeTotalTNPPrice = 15000,
                    TNP1 = 9000,
                    TNP2 = 6000,
                    PriceEpisodeInstalmentValue = 1000,
                    PriceEpisodePlannedInstalments = 12,
                    PriceEpisodeActualInstalments = null,
                    PriceEpisodeBalancePayment = 0,
                    PriceEpisodeFundLineType = testLearner.Course.FundingLineType,
                    PriceEpisodeBalanceValue = 0,
                    PriceEpisodeCompletionPayment = 3000,
                    PriceEpisodeContractType = testLearner.IsLevyLearner ? "Levy Contract" : "Non-Levy Contract",
                    PriceEpisodeOnProgPayment = 1000,
                    PriceEpisodePlannedEndDate = startDate.AddMonths(12),
                    PriceEpisodeAimSeqNumber = 1,
                    PriceEpisodeCumulativePMRs = int.MaxValue
                },
                PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>()
            };

            var learningValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeOnProgPayment",
            };
            Enumerable.Range(1, 12).ToList().ForEach(i => SetPeriodValue(i, learningValues, 1000));
            var completionEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeCompletionPayment",
            };
            SetPeriodValue(12, completionEarnings, 3000);
            var balancingEarnings = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeBalancePayment",
            };

            var sfaContributionValues = new PriceEpisodePeriodisedValues
            {
                AttributeName = "PriceEpisodeSFAContribPct"
            };
            Enumerable.Range(1, 12).ToList().ForEach(i => SetPeriodValue(i, sfaContributionValues, .9M));

            learner.PriceEpisodes = new List<PriceEpisode>(new[] { priceEpisode });
            priceEpisode.PriceEpisodePeriodisedValues.Add(learningValues);
            priceEpisode.PriceEpisodePeriodisedValues.Add(completionEarnings);
            priceEpisode.PriceEpisodePeriodisedValues.Add(balancingEarnings);
            priceEpisode.PriceEpisodePeriodisedValues.Add(sfaContributionValues);

            learner.LearningDeliveries = new List<LearningDelivery>(new[]
            {
                new LearningDelivery
                {
                    AimSeqNumber = 1,
                    LearningDeliveryValues = new LearningDeliveryValues
                    {
                        LearnStartDate = startDate,
                        LearnDelInitialFundLineType = testLearner.Course.FundingLineType,
                        PwayCode = testLearner.Course.PathwayCode,
                        FworkCode = testLearner.Course.FrameworkCode,
                        ProgType = testLearner.Course.ProgrammeType,
                        StdCode = testLearner.Course.StandardCode,
                        LearnAimRef = testLearner.Course.LearnAimRef,
                    }
                }
            });
            return learner;
        }

        private void SetPeriodValue(int period, PriceEpisodePeriodisedValues periodisedValues, decimal amount)
        {
            var periodProperty = periodisedValues.GetType().GetProperty("Period" + period);
            periodProperty?.SetValue(periodisedValues, amount);
        }

        protected async Task WaitForIt(Func<Task<bool>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            while (DateTime.Now < endTime)
            {
                if (await lookForIt())
                    return;
                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail(failText);
        }

        protected async Task WaitForIt(Func<Tuple<bool, string>> lookForIt, string failText)
        {
            var endTime = DateTime.Now.Add(Config.TimeToWait);
            var reason = "";
            while (DateTime.Now < endTime)
            {
                bool pass;
                (pass, reason) = lookForIt();
                if (pass) return;
                await Task.Delay(Config.TimeToPause);
            }
            Assert.Fail(failText + " - " + reason);
        }
    }
}