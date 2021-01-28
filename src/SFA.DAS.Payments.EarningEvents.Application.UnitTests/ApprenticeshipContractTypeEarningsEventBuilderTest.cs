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
using SFA.DAS.Payments.EarningEvents.Application.Services;
using SFA.DAS.Payments.EarningEvents.Application.UnitTests.Helpers;
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
        private  Mock<IRedundancyEarningService> redundancyEarningService;
        private const string filename = "Redundancy.json";
        private const string learnerRefNo = "01fm361845";

        [OneTimeSetUp]
        public void InitialiseDependencies()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
            redundancyEarningService = new Mock<IRedundancyEarningService>();
            redundancyEarningService
                .Setup(x => x.OriginalAndRedundancyEarningEventIfRequired(It.IsAny<ApprenticeshipContractTypeEarningsEvent>(), It.IsAny<List<byte>>()))
                .Returns((ApprenticeshipContractTypeEarningsEvent x, List<byte> y) => new List<ApprenticeshipContractTypeEarningsEvent>{ x });
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
                                PriceEpisodeTotalTNPPrice = 30000,
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

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,mapper);

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
                new ApprenticeshipContractTypeEarningsEventFactory(),redundancyEarningService.Object,mapper);

            var events = builder.Build(processLearnerCommand);

            events.Should().HaveCount(3);
        }

        [Test]
        public void CreatedTheCorrectNumberOfEarningEventsWhenLearningSupportIncluded()
        {
            var processLearnerCommand = CreateLearnerSubmissionWithLearningSupport();

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);

            var events = builder.Build(processLearnerCommand);

            events.Should().HaveCount(1);
            events.Single().OnProgrammeEarnings.Should().HaveCount(3);
            events.Single().OnProgrammeEarnings.Single(x => x.Type == OnProgrammeEarningType.Learning).Periods.Should().HaveCount(12);
            events.Single().IncentiveEarnings.Should().HaveCount(11);
            events.Single().IncentiveEarnings.Single(x => x.Type == IncentiveEarningType.LearningSupport).Periods.Should().HaveCount(12);
        }


        [Test]
        public void WhenALearnerIsMadeRedundant_AndIsReemployed_ThenThePriceEpisodesAreCorrect()
        {
            var processLearnerCommand = new ProcessLearnerCommand
            {
                Ukprn = 1,
                JobId = 1,
                CollectionPeriod = 4,
                CollectionYear = 2021,
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
                                LearnDelInitialFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                
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
                                LearnDelInitialFundLineType = "19+ Apprenticeship (Employer on App Service)",
                            }
                        }
                    },
                    PriceEpisodes = new List<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "25-17-07/11/2020",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2020-11-07"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Contract for services with the employer",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2021-08-01"),
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 0,
                                TNP3 = 5000,
                                PriceEpisodeCompleted = false,
                                PriceEpisodeTotalTNPPrice = 5000,
                                PriceEpisodeRedStatusCode = 0,
                                PriceEpisodeRedStartDate = null,
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 444.444444444444m,
                                    Period5 = 444.444444444444m,
                                    Period6 = 444.444444444444m,
                                    Period7 = 444.444444444444m,
                                    Period8 = 444.444444444444m,
                                    Period9 = 444.444444444444m,
                                    Period10 = 444.444444444444m,
                                    Period11 = 444.444444444444m,
                                    Period12 = 444.444444444444m,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeTotProgFunding",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 444.444444444444m,
                                    Period5 = 444.444444444444m,
                                    Period6 = 444.444444444444m,
                                    Period7 = 444.444444444444m,
                                    Period8 = 444.444444444444m,
                                    Period9 = 444.444444444444m,
                                    Period10 = 444.444444444444m,
                                    Period11 = 444.444444444444m,
                                    Period12 = 444.444444444444m,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeInstalmentsThisPeriod",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 1,
                                    Period5 = 1,
                                    Period6 = 1,
                                    Period7 = 1,
                                    Period8 = 1,
                                    Period9 = 1,
                                    Period10 = 1,
                                    Period11 = 1,
                                    Period12 = 1,
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
                                    AttributeName = "PriceEpisodeSFAContribPct",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 0.9m,
                                    Period5 = 0.9m,
                                    Period6 = 0.9m,
                                    Period7 = 0.9m,
                                    Period8 = 0.9m,
                                    Period9 = 0.9m,
                                    Period10 = 0.9m,
                                    Period11 = 0.9m,
                                    Period12 = 0.9m,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeESFAContribPct",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 0.9m,
                                    Period5 = 0.9m,
                                    Period6 = 0.9m,
                                    Period7 = 0.9m,
                                    Period8 = 0.9m,
                                    Period9 = 0.9m,
                                    Period10 = 0.9m,
                                    Period11 = 0.9m,
                                    Period12 = 0.9m,
                                },
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeLSFCash",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 0,
                                    Period4 = 150,
                                    Period5 = 150,
                                    Period6 = 150,
                                    Period7 = 150,
                                    Period8 = 150,
                                    Period9 = 150,
                                    Period10 = 150,
                                    Period11 = 150,
                                    Period12 = 150,
                                }
                            }
                        },
                        new PriceEpisode
                        {
                            PriceEpisodeIdentifier = "25-17-01/08/2020",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2020-08-01"),
                                PriceEpisodeActualEndDate = null,
                                PriceEpisodeFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                EpisodeEffectiveTNPStartDate = new DateTime(2017,05,1),
                                PriceEpisodeContractType = "Contract for services with the employer",
                                PriceEpisodeAimSeqNumber = 2,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2021-07-31"),
                                PriceEpisodePlannedInstalments = 12,
                                PriceEpisodeCompletionElement = 3000,
                                PriceEpisodeInstalmentValue = 1000,
                                TNP1 = 15000,
                                TNP2 = 0,
                                PriceEpisodeCompleted = false,
                                PriceEpisodeCumulativePMRs = 13,
                                PriceEpisodeCompExemCode = 14,
                                PriceEpisodeTotalTNPPrice = 15000,
                                PriceEpisodeRedStartDate = DateTime.Parse("2020-10-16"),
                                PriceEpisodeRedStatusCode = 1,
                            },
                            PriceEpisodePeriodisedValues = new List<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeInstalmentsThisPeriod",
                                    Period1 = 1,
                                    Period2 = 1,
                                    Period3 = 1,
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
                                    AttributeName = "PriceEpisodeLevyNonPayInd",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 1,
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
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 333.333333333338m,
                                    Period2 = 333.333333333338m,
                                    Period3 = 333.333333333338m,
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
                                    AttributeName = "PriceEpisodeSFAContribPct",
                                    Period1 = 0.9m,
                                    Period2 = 0.9m,
                                    Period3 = 1,
                                    Period4 = 0.9m,
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
                                    AttributeName = "PriceEpisodeESFAContribPct",
                                    Period1 = 0.9m,
                                    Period2 = 0.9m,
                                    Period3 = 1,
                                    Period4 = 0.9m,
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
                                    Period3 = 150,
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
                    }
                }
            };

            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(new ApprenticeshipContractTypeEarningsEventFactory(), 
                new RedundancyEarningService(new RedundancyEarningEventFactory(mapper)), mapper);

            var events = builder.Build(processLearnerCommand);
            events.Should().NotBeNull();
            events.Where(x => x.PriceEpisodes.Exists(y => y.Identifier == "25-17-01/08/2020"))
                .Single(x => x.GetType().IsAssignableFrom(typeof(ApprenticeshipContractType1RedundancyEarningEvent)))
                .OnProgrammeEarnings
                .Single(x => x.Type == OnProgrammeEarningType.Learning)
                .Periods
                .Should().HaveCount(1);
            events.Where(x => x.PriceEpisodes.Exists(y => y.Identifier == "25-17-01/08/2020"))
                .Single(x => x.GetType().IsAssignableFrom(typeof(ApprenticeshipContractType1EarningEvent)))
                .OnProgrammeEarnings
                .Single(x => x.Type == OnProgrammeEarningType.Learning)
                .Periods
                .Should().HaveCount(11);
            events.Where(x => x.PriceEpisodes.Exists(y => y.Identifier == "25-17-01/08/2020"))
                .Single(x => x.GetType().IsAssignableFrom(typeof(ApprenticeshipContractType1RedundancyEarningEvent)))
                .IncentiveEarnings
                .Single(x => x.Type == IncentiveEarningType.LearningSupport)
                .Periods
                .Should().HaveCount(1);
            events.Where(x => x.PriceEpisodes.Exists(y => y.Identifier == "25-17-01/08/2020"))
                .Single(x => x.GetType().IsAssignableFrom(typeof(ApprenticeshipContractType1EarningEvent)))
                .IncentiveEarnings
                .Single(x => x.Type == IncentiveEarningType.LearningSupport)
                .Periods
                .Should().HaveCount(11);
        }

        [Test]
        public void RedundantLearner_shouldSplitEarningEventAtPeriodWhereRedundancyTakesPlace()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);
            var processLearnerCommand = FileHelpers.CreateFromFile(filename, learnerRefNo);

            redundancyEarningService.Setup(x => x.OriginalAndRedundancyEarningEventIfRequired(It.IsAny<ApprenticeshipContractType2EarningEvent>(), It.IsAny<List<byte>>()))
                .Returns(new List<ApprenticeshipContractTypeEarningsEvent>());

            builder.Build(processLearnerCommand);

            redundancyEarningService.Verify(x => x.OriginalAndRedundancyEarningEventIfRequired(It.IsAny<ApprenticeshipContractType2EarningEvent>(), It.IsAny<List<byte>>()));
        }

        [Test]
        public void CalculateRedundancyPeriods_ReturnsEmptyListWhenNoRedundancy()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);

            var actual = builder.CalculateRedundancyPeriods(new List<PriceEpisode>
            {
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 0
                    }
                }
            });

            actual.Should().BeEmpty();
        }

        [Test]
        public void CalculateRedundancyPeriods_ReturnsPeriodsFromRedundancyDateToEndOfYear()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);

            var actual = builder.CalculateRedundancyPeriods(new List<PriceEpisode>
            {
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 1,
                        EpisodeStartDate = new DateTime(2020, 8, 1),
                        PriceEpisodeRedStartDate = new DateTime(2020, 10, 2)
                    }
                }
            });

            actual.Should().HaveCount(10);
            actual.Should().BeInAscendingOrder();
            actual[0].Should().Be(3);
        }

        [Test]
        public void CalculateRedundancyPeriods_ReturnsPeriodsFromRedundancyDateToNextNonRedundantPriceEpisode()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);

            var actual = builder.CalculateRedundancyPeriods(new List<PriceEpisode>
            {
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 1,
                        EpisodeStartDate = new DateTime(2020, 8, 1),
                        PriceEpisodeRedStartDate = new DateTime(2020, 10, 2)
                    }
                },
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 0,
                        EpisodeStartDate = new DateTime(2020, 11, 14),
                    }
                }
            });

            actual.Should().HaveCount(1);
            actual[0].Should().Be(3);
        }

        [Test]
        public void CalculateRedundancyPeriods_ReturnsPeriodsCorrectlyInComplexRedundancyScenario()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(), redundancyEarningService.Object,
                mapper);

            var actual = builder.CalculateRedundancyPeriods(new List<PriceEpisode>
            {
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 1,
                        EpisodeStartDate = new DateTime(2020, 8, 1),
                        PriceEpisodeRedStartDate = new DateTime(2020, 10, 2)
                    }
                },
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 1,
                        EpisodeStartDate = new DateTime(2020, 11, 14),
                        PriceEpisodeRedStartDate = new DateTime(2021, 1, 17),
                    }
                },
                new PriceEpisode
                {
                    PriceEpisodeValues = new PriceEpisodeValues
                    {
                        PriceEpisodeRedStatusCode = 0,
                        EpisodeStartDate = new DateTime(2021, 4, 9),
                    }
                }
            });

            actual.Should().HaveCount(4);
            actual.Should().Contain(3);
            actual.Should().Contain(6);
            actual.Should().Contain(7);
            actual.Should().Contain(8);
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


    }
}
