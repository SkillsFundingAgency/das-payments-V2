using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{
    [TestFixture]
    public class Act1MappingTest
    {

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfile<EarningsEventProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void TestAct1EarningIsDetected()
        {
            // arrange
            var fm36Learner = new FM36Learner
            {
                PriceEpisodes = new List<PriceEpisode>()
                {
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "1",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            PriceEpisodeContractType = "ContractWithSfa"
                        }
                    }
                }
            };

            var processLearnerCommand = new ProcessLearnerCommand
            {
                Learner = fm36Learner,
                CollectionYear = "1819",
                Ukprn = 12345,
                JobId = 69,
                CollectionPeriod = 1,
                IlrSubmissionDateTime = DateTime.UtcNow,
                SubmissionDate = DateTime.UtcNow
            };

            var learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);

            // act
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType1EarningEvent>(learningAim);

            // assert

        }
    }
}
