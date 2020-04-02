using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class ApprenticeshipContractTypeEarningsEventBuilderTest
    {

        private IMapper mapper;
        private  Mock<IRedundancyEarningSplitter> redundancyEarningSplitterMock;
        private const string filename = "Redundancy.json";
        private const string learnerRefNo = "01fm361845";

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
            redundancyEarningSplitterMock = new Mock<IRedundancyEarningSplitter>();
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
                                PwayCode = 500,
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

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningSplitterMock.Object,mapper);

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
                new ApprenticeshipContractTypeEarningsEventFactory(),redundancyEarningSplitterMock.Object,mapper);

            var events = builder.Build(processLearnerCommand);

            events.Should().HaveCount(3);
        }

        [Test]
        public void CreatedTheCorrectNumberOfEarningEventsWhenLearningSupportIncluded()
        {
            var processLearnerCommand = CreateLearnerSubmissionWithLearningSupport();

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningSplitterMock.Object,
                mapper);

            var events = builder.Build(processLearnerCommand);

            events.Should().HaveCount(1);
            events.Single().OnProgrammeEarnings.Should().HaveCount(3);
            events.Single().OnProgrammeEarnings.Single(x => x.Type == OnProgrammeEarningType.Learning).Periods.Should().HaveCount(12);
            events.Single().IncentiveEarnings.Should().HaveCount(11);
            events.Single().IncentiveEarnings.Single(x => x.Type == IncentiveEarningType.LearningSupport).Periods.Should().HaveCount(12);
        }


        [Test]
        public void RedundantLearner_shouldSplitEarningEventAtPeriodWhereRedundancyTakesPlace()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), new RedundancyEarningSplitter(new RedundancyEarningEventFactory(mapper)),
                mapper);
            var processLearnerCommand = Helpers.FileHelpers.CreateFromFile(filename, learnerRefNo);
            var redundancyDate = processLearnerCommand.Learner.PriceEpisodes.First().PriceEpisodeValues
                .PriceEpisodeRedStartDate.Value;

            var redundancyPeriod = GetPeriodFromDate(redundancyDate);

            var results = builder.Build(processLearnerCommand);

            results.Should().HaveCount(2);
            results.First().Should().BeOfType<ApprenticeshipContractType2EarningEvent>();
            //results.First().OnProgrammeEarnings.Should().SatisfyRespectively(ope =>
            //    ope.Periods.Where(p => p.Period >= redundancyPeriod).All(p => p.Amount == 0));
            results.First().OnProgrammeEarnings.All(ope => ope.Periods.Where(p => p.Period >= redundancyPeriod)
                .All(p => p.Amount == 0));

            results.Last().OnProgrammeEarnings.All(ope => ope.Periods.Where(p => p.Period < redundancyPeriod)
                .All(p => p.Amount == 1000m));
            results.Last().OnProgrammeEarnings.All(ope => ope.Periods.Where(p => p.Period >= redundancyPeriod)
                .All(p => p.SfaContributionPercentage == 1m));



        }

        private static ProcessLearnerCommand CreateLearnerSubmissionWithLearningSupport()
        {
            return new ProcessLearnerCommand
            {
                Ukprn = 1,
                JobId = 1,
                CollectionPeriod = 3,
                CollectionYear = 1920,
                IlrSubmissionDateTime = DateTime.Today,
                SubmissionDate = DateTime.Today,
                Learner = new FM36Learner
                {
                    LearnRefNumber = "learner-a",
                    ULN = 1234678,
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "20-593-1-06/08/2019",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-06T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = DateTime.Parse("2019-10-05T00:00:00+00:00"),
                                PriceEpisodeFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Contract for services with the employer",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2020-08-06T00:00:00+00:00"),
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = false,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 30000
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 750,
                                    Period2 = 750,
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
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeLSFCash",
                                    Period1 = 150,
                                    Period2 = 150,
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
                                }
                            }
                        },
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "20-593-1-06/10/2019",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-10-06T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = DateTime.Parse("2020-07-31T00:00:00+00:00"),
                                PriceEpisodeFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Contract for services with the employer",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2020-08-06T00:00:00+00:00"),
                                PriceEpisodePlannedInstalments = 10,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 15000,
                                PriceEpisodeCompleted = false,
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
                                    Period3 = 390,
                                    Period4 = 390,
                                    Period5 = 390,
                                    Period6 = 390,
                                    Period7 = 390,
                                    Period8 = 390,
                                    Period9 = 390,
                                    Period10 = 390,
                                    Period11 = 390,
                                    Period12 = 390,
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
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeLSFCash",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 150,
                                    Period4 = 150,
                                    Period5 = 150,
                                    Period6 = 150,
                                    Period7 = 150,
                                    Period8 = 150,
                                    Period9 = 150,
                                    Period10 = 150,
                                    Period11 = 150,
                                    Period12 = 150,
                                },
                            }
                        }
                    },
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
                                LearnDelInitialFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                LearnStartDate = DateTime.Parse("2019-08-06T00:00:00+00:00")
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "LearnSuppFundCash",
                                    Period1 = 150,
                                    Period2 = 150,
                                    Period3 = 150,
                                    Period4 = 150,
                                    Period5 = 150,
                                    Period6 = 150,
                                    Period7 = 150,
                                    Period8 = 150,
                                    Period9 = 150,
                                    Period10 = 150,
                                    Period11 = 150,
                                    Period12 = 150
                                }
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
                                LearnDelInitialFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                LearnStartDate = DateTime.Parse("2019-08-06T00:00:00+00:00")
                            },
                            LearningDeliveryPeriodisedValues = new List<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 39.25m,
                                    Period2 = 39.25m,
                                    Period3 = 39.25m,
                                    Period4 = 39.25m,
                                    Period5 = 39.25m,
                                    Period6 = 39.25m,
                                    Period7 = 39.25m,
                                    Period8 = 39.25m,
                                    Period9 = 39.25m,
                                    Period10 = 39.25m,
                                    Period11 = 39.25m,
                                    Period12 = 39.25m,
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
                    }
                }
            };
        }

        
        private byte GetPeriodFromDate(DateTime date)
        {
            byte period;
            var month = date.Month;

            if (month < 8)
            {
                period = (byte) (month + 5);
            }
            else
            {
                period = (byte) (month - 7);
            }
            return period;
        }
    }
}
