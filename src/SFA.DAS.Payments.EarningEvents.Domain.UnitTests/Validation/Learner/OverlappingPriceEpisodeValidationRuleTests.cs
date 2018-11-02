using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests.Validation.Learner
{
    [TestFixture]
    public class OverlappingPriceEpisodeValidationRuleTests
    {
        [Test]
        public void Validation_Fails_If_There_Are_Overlapping_Price_Episodes()
        {
            var learner = new FM36Learner
            {
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-1",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            EpisodeStartDate = DateTime.Today.AddMonths(-2),
                            PriceEpisodePlannedEndDate = DateTime.Today.AddDays(-30),
                        }
                    },
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-2",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            EpisodeStartDate = DateTime.Today.AddDays(-35),
                            PriceEpisodePlannedEndDate = DateTime.Today,
                        }
                    }
                }
            };

            var rule = new OverlappingPriceEpisodeValidationRule();
            var result = rule.IsValid(learner);

            Assert.IsTrue(result.Failed,result.FailureReason);
        }
    }
}