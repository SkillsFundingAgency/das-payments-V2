using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{

    [TestFixture]
    public class MappingTest
    {
        private FM36Learner fm36Learner;
        private ProcessLearnerCommand processLearnerCommand;
        private IntermediateLearningAim learningAim;
        
        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<EarningsEventProfile>();
            });
            Mapper.AssertConfigurationIsValid();
        }

        [SetUp]
        public void SetUp()
        {
            fm36Learner = new FM36Learner
            {
                LearnRefNumber = "learner-a",
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValues = new LearningDeliveryValues
                        {
                            LearnAimRef = "ZPROG001",
                            StdCode = 100,
                            FworkCode = 200,
                            ProgType = 300,
                            PwayCode = 400,
                            LearnDelInitialFundLineType = "Funding Line Type",
                        }
                    }
                },
                PriceEpisodes = new List<PriceEpisode>
                {
                    new PriceEpisode
                    {
                        PriceEpisodeIdentifier = "pe-1",
                        PriceEpisodeValues = new PriceEpisodeValues
                        {

                            EpisodeStartDate = DateTime.Today.AddMonths(-12),
                            PriceEpisodePlannedEndDate = DateTime.Today,
                            PriceEpisodeActualEndDate = DateTime.Today,
                            PriceEpisodePlannedInstalments = 12,
                            PriceEpisodeCompletionElement = 3000,
                            PriceEpisodeInstalmentValue = 1000,
                            TNP1 = 15000,
                            TNP2 = 15000,
                            PriceEpisodeCompleted = true,
                        },
                        PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                        {
                            new PriceEpisodePeriodisedValues
                            {
                                AttributeName = "PriceEpisodeOnProgPayment",
                                Period1 = 1000,
                                Period2 = 1000,
                                Period3 = 1000,
                                Period4 = 1000,
                                Period5 = 1000,
                                Period6 = 1000,
                                Period7 = 1000,
                                Period8 = 1000,
                                Period9 = 1000,
                                Period10 = 1000,
                                Period11 = 1000,
                                Period12 = 1000,
                            },
                            new PriceEpisodePeriodisedValues
                            {
                                AttributeName = "PriceEpisodeCompletionPayment",
                                Period1 = 0,
                                Period2 = 0,
                                Period3 = 0,
                                Period4 = 0,
                                Period5 = 0,
                                Period6 = 0,
                                Period7 = 0,
                                Period8 = 0,
                                Period9 = 0,
                                Period10 = 0,
                                Period11 = 0,
                                Period12 = 3000,
                            },
                        }
                    }
                }
            };

            processLearnerCommand = new ProcessLearnerCommand
            {
                Learner = fm36Learner,
                CollectionYear = "1819",
                Ukprn = 12345,
                JobId = 69,
                CollectionPeriod = 1,
                IlrSubmissionDateTime = DateTime.UtcNow,
                SubmissionDate = DateTime.UtcNow
            };

            learningAim = new IntermediateLearningAim(processLearnerCommand, fm36Learner.PriceEpisodes, fm36Learner.LearningDeliveries[0]);
        }

        [Test]
        public void Maps_Ukprn()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Ukprn.Should().Be(processLearnerCommand.Ukprn);
        }

        [Test]
        public void Maps_JobId()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.JobId.Should().Be(processLearnerCommand.JobId);
        }

        [Test]
        public void Maps_Collection_Year()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.CollectionYear.Should().Be("1819");
        }

        [Test]
        public void Maps_Collection_Period()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.CollectionPeriod.Should().NotBeNull();
            earningEvent.CollectionPeriod.Period.Should().Be(1);
        }

        [Test]
        public void Maps_IlrSubmissionTime()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.IlrSubmissionDateTime.Should().Be(processLearnerCommand.IlrSubmissionDateTime);
        }

        [Test]
        public void Maps_Learner()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Learner.Should().NotBeNull();
            earningEvent.Learner.ReferenceNumber.Should().Be(fm36Learner.LearnRefNumber);
        }

        [Test]
        public void Maps_Price_Episodes()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.PriceEpisodes.Should().NotBeEmpty();
            earningEvent.PriceEpisodes.First().Identifier.Should().Be("pe-1");
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice1.Should().Be(15000);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice2.Should().Be(15000);
            earningEvent.PriceEpisodes.First().CompletionAmount.Should().Be(3000);
            earningEvent.PriceEpisodes.First().InstalmentAmount.Should().Be(1000);
            earningEvent.PriceEpisodes.First().NumberOfInstalments.Should().Be(12);
            earningEvent.PriceEpisodes.First().TotalNegotiatedPrice3.Should().BeNull();
        }

        [Test]
        public void Maps_Price_Episode_Periodised_Values_To_Earning_Periods()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.OnProgrammeEarnings.Should().NotBeNullOrEmpty();
            earningEvent.OnProgrammeEarnings.Should().HaveCount(2);
            var learnings = earningEvent.OnProgrammeEarnings.FirstOrDefault(x => x.Type == OnProgrammeEarningType.Learning);
            learnings.Should().NotBeNull();
            learnings.Periods.Count.Should().Be(12);
            learnings.Periods.All(period => period.Amount == 1000 && period.PriceEpisodeIdentifier == "pe-1").Should().BeTrue();
            var completion = earningEvent.OnProgrammeEarnings.FirstOrDefault(x => x.Type == OnProgrammeEarningType.Completion);
            completion.Should().NotBeNull();
            completion.Periods.Count.Should().Be(12);
            completion.Periods.Any(x => x.Period == 12 && x.Amount == 3000).Should().BeTrue();
        }

        [Test]
        public void Maps_On_Programme_Earnings()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var learning =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Learning);
            learning.Should().NotBeNull();
            learning.Periods.Should().HaveCount(12);
            learning.Periods.All(period => period.Amount == 1000).Should().BeTrue();
        }

        [Test]
        public void Maps_Completion_Earnings()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            var completion =
                earningEvent.OnProgrammeEarnings.FirstOrDefault(earnings =>
                    earnings.Type == OnProgrammeEarningType.Completion);
            completion.Should().NotBeNull();
            completion.Periods.Should().HaveCount(12);
            var completionPeriod = completion.Periods.FirstOrDefault(p => p.Period == 12);
            completionPeriod.Should().NotBeNull();
            completionPeriod.Amount.Should().Be(3000);
        }

        [Test]
        public void Maps_LearningAim()
        {
            var earningEvent = Mapper.Instance.Map<IntermediateLearningAim, ApprenticeshipContractType2EarningEvent>(learningAim);
            earningEvent.Should().NotBeNull();
            earningEvent.LearningAim.Reference.Should().Be("ZPROG001");
            earningEvent.LearningAim.FrameworkCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0]
                .LearningDeliveryValues.FworkCode);
            earningEvent.LearningAim.StandardCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0]
                .LearningDeliveryValues.StdCode);
            earningEvent.LearningAim.ProgrammeType.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0]
                .LearningDeliveryValues.ProgType);
            earningEvent.LearningAim.PathwayCode.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0]
                .LearningDeliveryValues.PwayCode);
            earningEvent.LearningAim.FundingLineType.Should().Be(processLearnerCommand.Learner.LearningDeliveries[0]
                .LearningDeliveryValues.LearnDelInitialFundLineType);
        }
    }
}