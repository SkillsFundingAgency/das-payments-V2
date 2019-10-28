using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using FluentAssertions;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class ApprenticeshipContractTypeEarningsEventBuilderTest
    {

        private IMapper mapper;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
        }


        [Test]
        public void BuildFundingLineTypeCorrectly()
        {
            var processLearnerCommand = new ProcessLearnerCommand
            {
                Ukprn = 1,
                JobId = 1,
                CollectionPeriod = 1,
                CollectionYear = 1920,
                IlrSubmissionDateTime = DateTime.Today,
                SubmissionDate = DateTime.Today,
                Learner = new FM36Learner
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
                                LearnDelInitialFundLineType = "Funding Line Type 1",
                                LearnStartDate = DateTime.Today.AddDays(-5)
                            }
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 2,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "ZPROG001",
                                StdCode = 100,
                                FworkCode = 200,
                                ProgType = 300,
                                PwayCode = 400,
                                LearnDelInitialFundLineType = "Funding Line Type 2",
                                LearnStartDate = DateTime.Today.AddDays(-6)
                            },
                        }
                    },
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "pe-1",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-01"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship Non-Levy Contract (procured)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Non-Levy Contract",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2019-10-01"),
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = true,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
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
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeBalancePayment",
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
                        },
                         new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "pe-2",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = new DateTime(2019,10,1),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship Levy Contract (procured)",
                                EpisodeEffectiveTNPStartDate = new DateTime(2017,05,1),
                                PriceEpisodeContractType = "Levy Contract",
                                PriceEpisodeAimSeqNumber = 2,
                                PriceEpisodePlannedEndDate = new DateTime(2019,12,1),
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = true,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
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
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeBalancePayment",
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
                        },
                    }
                }
            };

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(new ApprenticeshipContractTypeEarningsEventFactory(), mapper);

            var events = builder.Build(processLearnerCommand);
            events.Should().NotBeNull();
            events[0].PriceEpisodes[0].FundingLineType.Should().Be(processLearnerCommand.Learner.PriceEpisodes[0].PriceEpisodeValues.PriceEpisodeFundLineType);
            events[1].PriceEpisodes[0].FundingLineType.Should().Be(processLearnerCommand.Learner.PriceEpisodes[1].PriceEpisodeValues.PriceEpisodeFundLineType);
        }


        [Test]
        public void CreatedTheCorrectNumberOfEarningEvents()
        {
            var processLearnerCommand = new ProcessLearnerCommand
            {
                Ukprn = 1,
                JobId = 1,
                CollectionPeriod = 1,
                CollectionYear = 1920,
                IlrSubmissionDateTime = DateTime.Today,
                SubmissionDate = DateTime.Today,
                Learner = new FM36Learner
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
                                LearnDelInitialFundLineType = "Funding Line Type 1",
                                LearnStartDate = DateTime.Today.AddDays(-5)
                            }
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 2,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "M&E2",
                                StdCode = 100,
                                FworkCode = 200,
                                ProgType = 300,
                                PwayCode = 400,
                                LearnDelInitialFundLineType = "Funding Line Type 2",
                                LearnStartDate = DateTime.Today.AddDays(-10)
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 100,
                                    Period2 = 100,
                                    Period3 = 100,
                                    Period4 = 0,
                                    Period5 = 0,
                                    Period6 = 0,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
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
                                    Period12 = 0,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "LearnSuppFundCash",
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
                                    Period12 = 0,
                                },
                            },
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 3,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "M&E3",
                                StdCode = 100,
                                FworkCode = 200,
                                ProgType = 300,
                                PwayCode = 400,
                                LearnDelInitialFundLineType = "Funding Line Type 2",
                                LearnStartDate = DateTime.Today.AddDays(-10)
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 100,
                                    Period5 = 100,
                                    Period6 = 100,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
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
                                    Period12 = 0,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "LearnSuppFundCash",
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
                                    Period12 = 0,
                                },
                            },
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 4,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "M&E4",
                                StdCode = 100,
                                FworkCode = 200,
                                ProgType = 300,
                                PwayCode = 400,
                                LearnDelInitialFundLineType = "Funding Line Type 2",
                                LearnStartDate = DateTime.Today.AddDays(-10)
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 0,
                                    Period5 = 0,
                                    Period6 = 0,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 100,
                                    Period11 = 100,
                                    Period12 = 100,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
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
                                    Period12 = 0,
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "LearnSuppFundCash",
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
                                    Period12 = 0,
                                },
                            },
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 5,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "ZPROG001",
                                StdCode = 100,
                                FworkCode = 200,
                                ProgType = 500,
                                PwayCode = 400,
                                LearnDelInitialFundLineType = "Funding Line Type 2",
                                LearnStartDate = DateTime.Today.AddDays(-5)
                            }
                        },
                    },
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "pe-1",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-01T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship Non-Levy Contract (procured)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Non-Levy Contract",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Today,
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = true,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 1000,
                                    Period2 = 1000,
                                    Period3 = 1000,
                                    Period4 = 0,
                                    Period5 = 0,
                                    Period6 = 0,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0,
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
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeBalancePayment",
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
                        },
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "pe-2",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-01T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship Non-Levy Contract (procured)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Levy Contract",
                                PriceEpisodeAimSeqNumber = 5,
                                PriceEpisodePlannedEndDate = DateTime.Today,
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = true,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 1000,
                                    Period5 = 1000,
                                    Period6 = 1000,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0,
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
                                    Period12 = 0,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeBalancePayment",
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
                                    Period12 = 0,
                                },
                            }
                        },
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "pe-3",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-01T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship Non-Levy Contract (procured)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Non-Levy Contract",
                                PriceEpisodeAimSeqNumber = 5,
                                PriceEpisodePlannedEndDate = DateTime.Today,
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = true,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 0,
                                    Period5 = 0,
                                    Period6 = 0,
                                    Period7 = 1000,
                                    Period8 = 1000,
                                    Period9 = 1000,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0,
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
                                    Period12 = 0,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeBalancePayment",
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
                                    Period12 = 0,
                                },
                            }
                        },
                    }
                }
            };

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(),
                mapper);

            var events = builder.Build(processLearnerCommand);

            events.Should().HaveCount(3);
        }
    }
}
