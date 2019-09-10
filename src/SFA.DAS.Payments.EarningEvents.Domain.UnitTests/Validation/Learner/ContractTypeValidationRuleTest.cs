using System;
using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner.Rules;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests.Validation.Learner
{
    [TestFixture]
    public class ContractTypeValidationRuleTest
    {
        [Test]
        [TestCase("Levy Contract", false)]
        [TestCase("Non-Levy Contract", false)]
        [TestCase("Contract for services with the employer", false)]
        [TestCase("Contract for services with the ESFA", false)]
        [TestCase("None", true)]
        public void ValidationChecksContractType(string contractType, bool expectedFailed)
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
                            PriceEpisodeActualEndDate = DateTime.Today.AddDays(-30),
                            PriceEpisodeContractType = contractType
                        }
                    },
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-2",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            EpisodeStartDate = DateTime.Today.AddDays(-35),
                            PriceEpisodePlannedEndDate = DateTime.Today,
                            PriceEpisodeContractType = "Levy Contract",
                        }
                    }
                }
            };

            var rule = new ContractTypeValidationRule();
            Assert.AreEqual(expectedFailed, rule.IsValid(learner).Failed);
        }
    }
}
