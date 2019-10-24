using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using PriceEpisode = ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class FunctionalSkillEarningEventBuilderTest
    {
        private IMapper mapper;
        private ProcessLearnerCommand learnerSubmission;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
        }

        [SetUp]
        public void SetUp()
        {
            learnerSubmission = new ProcessLearnerCommand
            {
                Ukprn = 1000,
                CollectionPeriod = 1,
                CollectionYear = 1920,
                SubmissionDate = DateTime.Today,
                JobId = 1,
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                        new LearningDelivery
                        {
                            AimSeqNumber = 1,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "5011397X",
                                LearnDelInitialFundLineType = "16-18 Apprenticeship (Employer on App Service)",
                            },
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 1,
                                    Period2 = 2,
                                    Period3 = 3,
                                    Period4 = 4,
                                    Period5 = 5,
                                    Period6 = 6,
                                    Period7 = 7,
                                    Period8 = 8,
                                    Period9 = 9,
                                    Period10 = 10,
                                    Period11 = 11,
                                    Period12 = 12
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 =  "None",
                                    Period2 =  "None",
                                    Period3 =  "None",
                                    Period4 =  "None",
                                    Period5 =  "None",
                                    Period6 =  "None",
                                    Period7 =  "None",
                                    Period8 =  "None",
                                    Period9 =  "None",
                                    Period10 = "None",
                                    Period11 = "None",
                                    Period12 = "None",
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName= "FundLineType",
                                    Period1 = "none",
                                    Period2 = "none",
                                    Period3 = "none",
                                    Period4 = "none",
                                    Period5 = "none",
                                    Period6 = "none",
                                    Period7 = "none",
                                    Period8 = "none",
                                    Period9 = "none",
                                    Period10 ="none",
                                    Period11 ="none",
                                    Period12 ="none",
                                }
                            }
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 2,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "5022394X",
                                LearnDelInitialFundLineType = "16-18 Apprenticeship (Employer on App Service)",
                            },
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 1,
                                    Period2 = 2,
                                    Period3 = 3,
                                    Period4 = 4,
                                    Period5 = 5,
                                    Period6 = 6,
                                    Period7 = 7,
                                    Period8 = 8,
                                    Period9 = 9,
                                    Period10 = 10,
                                    Period11 = 11,
                                    Period12 = 12
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues 
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "None",
                                    Period2 =  "None",
                                    Period3 =  "None",
                                    Period4 =  "None",
                                    Period5 =  "None",
                                    Period6 =  "None",
                                    Period7 =  "None",
                                    Period8 =  "None",
                                    Period9 =  "None",
                                    Period10 = "None",
                                    Period11 = "None",
                                    Period12 = "None",
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName= "FundLineType",
                                    Period1 = "none",
                                    Period2 = "none",
                                    Period3 = "none",
                                    Period4 = "none",
                                    Period5 = "none",
                                    Period6 = "none",
                                    Period7 = "none",
                                    Period8 = "none",
                                    Period9 = "none",
                                    Period10 ="none",
                                    Period11 ="none",
                                    Period12 ="none",
                                }
                            }
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 3,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "ZPROG001"
                            },
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
                                    Period1 = 0.0m,
                                    Period2 = 0.0m,
                                    Period3 = 0.0m,
                                    Period4 = 0.0m,
                                    Period5 = 0.0m,
                                    Period6 = 0.0m,
                                    Period7 = 0.0m,
                                    Period8 = 0.0m,
                                    Period9 = 0.0m,
                                    Period10 = 0.0m,
                                    Period11 = 0.0m,
                                    Period12 = 0.0m
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {

                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "Levy Contract",
                                    Period2 = "Levy Contract",
                                    Period3 = "Levy Contract",
                                    Period4 = "Levy Contract",
                                    Period5 = "Levy Contract",
                                    Period6 = "Levy Contract",
                                    Period7 = "Levy Contract",
                                    Period8 = "Levy Contract",
                                    Period9 = "Levy Contract",
                                    Period10 = "Levy Contract",
                                    Period11 = "Levy Contract",
                                    Period12 = "Levy Contract"
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName= "FundLineType",
                                    Period1 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period2 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period3 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period4 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period5 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period6 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period7 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period8 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period9 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period10 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period11 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period12 = "16-18 Apprenticeship (Employer on App Service)"
                                }

                            }
                        },
                        new LearningDelivery
                        {
                            AimSeqNumber = 4,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "5011394X",
                                LearnDelInitialFundLineType = "16-18 Apprenticeship (Employer on App Service)",
                            },
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngOnProgPayment",
                                    Period1 = 1,
                                    Period2 = 2,
                                    Period3 = 3,
                                    Period4 = 4,
                                    Period5 = 5,
                                    Period6 = 6,
                                    Period7 = 7,
                                    Period8 = 8,
                                    Period9 = 9,
                                    Period10 = 10,
                                    Period11 = 11,
                                    Period12 = 12
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "Non-Levy Contract",
                                    Period2 = "Non-Levy Contract",
                                    Period3 = "Non-Levy Contract",
                                    Period4 = "Non-Levy Contract",
                                    Period5 = "None",
                                    Period6 = "None",
                                    Period7 = "Levy Contract",
                                    Period8 = "Levy Contract",
                                    Period9 = "Levy Contract",
                                    Period10 = "Levy Contract",
                                    Period11 = "Levy Contract",
                                    Period12 = "Levy Contract"
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName= "FundLineType",
                                    Period1 = "16-18 Apprenticeship Non-Levy Contract (procured)",
                                    Period2 = "16-18 Apprenticeship Non-Levy Contract (procured)",
                                    Period3 = "16-18 Apprenticeship Non-Levy Contract (procured)",
                                    Period4 = "16-18 Apprenticeship Non-Levy Contract (procured)",
                                    Period5 = "none",
                                    Period6 = "none",
                                    Period7 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period8 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period9 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period10 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period11 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period12 = "16-18 Apprenticeship (Employer on App Service)"
                                }
                            }
                        },

                    },
                    PriceEpisodes = new EditableList<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Today,
                                PriceEpisodeAimSeqNumber = 3,
                                PriceEpisodeContractType = "Levy Contract",
                                TNP1 = 2500,
                                PriceEpisodeFundLineType ="16-18 Apprenticeship (Employer on App Service)",
                            },
                            PriceEpisodePeriodisedValues = new EditableList<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 10,
                                    Period2 = 20
                                }
                            }
                        }
                    }
                }
            };
        }

        [Test]
        public void TestBuild()
        {
            // arrange
            var expectedResult = new Act1FunctionalSkillEarningsEvent
            {
                Earnings = new List<FunctionalSkillEarning>().AsReadOnly()
            };
            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            var learnerSubmission = new ProcessLearnerCommand
            {
                CollectionPeriod = 1,
                CollectionYear = 1920,
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                        new LearningDelivery {AimSeqNumber = 1, LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "ZPROG001"}},
                        new LearningDelivery
                        {
                            AimSeqNumber = 2,
                            LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "M&E"},
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
                                    Period1 = 1,
                                    Period2 = 2
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "Levy Contract",
                                    Period2 = "Levy Contract",
                                    Period3 = "Levy Contract",
                                    Period4 = "Levy Contract",
                                    Period5 = "Levy Contract",
                                    Period6 = "Levy Contract",
                                    Period7 = "Levy Contract",
                                    Period8 = "Levy Contract",
                                    Period9 = "Levy Contract",
                                    Period10 = "Levy Contract",
                                    Period11 = "Levy Contract",
                                    Period12 = "Levy Contract"
                                }
                            }
                        }
                    },
                    PriceEpisodes = new EditableList<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Today,
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodeContractType = "Levy Contract",
                            },
                            PriceEpisodePeriodisedValues = new EditableList<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 10,
                                    Period2 = 20
                                }
                            }
                        }
                    }
                }
            };

            // act
            var events = builder.Build(learnerSubmission);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);

        }

        [Test]
        public void FunctionalSkillBuild()
        {
            // arrange
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            var earningPeriods = new List<EarningPeriod>
            {
                new EarningPeriod {Period = 1, Amount = 1},
                new EarningPeriod {Period = 2, Amount = 2},
                new EarningPeriod {Period = 3, Amount = 3},
                new EarningPeriod {Period = 4, Amount = 4},
                new EarningPeriod {Period = 5, Amount = 5},
                new EarningPeriod {Period = 6, Amount = 6},
                new EarningPeriod {Period = 7, Amount = 7},
                new EarningPeriod {Period = 8, Amount = 8},
                new EarningPeriod {Period = 9, Amount = 9},
                new EarningPeriod {Period = 10, Amount = 10},
                new EarningPeriod {Period = 11, Amount = 11},
                new EarningPeriod {Period = 12, Amount = 12},
            };

            var earnings = new List<FunctionalSkillEarning>()
            {
                new FunctionalSkillEarning
                {
                    Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                    Periods = earningPeriods.AsReadOnly()
                },
                new FunctionalSkillEarning
                {
                    Type = FunctionalSkillType.BalancingMathsAndEnglish,
                    Periods = earningPeriods.AsReadOnly()
                }
            };

            var expectedResult = new Act1FunctionalSkillEarningsEvent
            {
                Earnings = earnings.AsReadOnly()
            };

            mockMapper.Setup(m => m.Map<Act1FunctionalSkillEarningsEvent>(It.IsAny<IntermediateLearningAim>())).Returns(expectedResult).Verifiable();
            var builder = new FunctionalSkillEarningEventBuilder(mockMapper.Object);
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                        new LearningDelivery
                        {
                            AimSeqNumber = 1,
                            LearningDeliveryValues = new LearningDeliveryValues
                            {
                                LearnAimRef = "M&E"
                            },
                            LearningDeliveryPeriodisedValues = new EditableList<LearningDeliveryPeriodisedValues>
                            {
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "MathEngBalPayment",
                                    Period1 = 1,
                                    Period2 = 2,
                                    Period3 = 3,
                                    Period4 = 4,
                                    Period5 = 5,
                                    Period6 = 6,
                                    Period7 = 7,
                                    Period8 = 8,
                                    Period9 = 9,
                                    Period10 = 10,
                                    Period11 = 11,
                                    Period12 = 12
                                }
                            },
                            LearningDeliveryPeriodisedTextValues = new EditableList<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "Levy Contract",
                                    Period2 = "Levy Contract",
                                    Period3 = "Levy Contract",
                                    Period4 = "Levy Contract",
                                    Period5 = "Levy Contract",
                                    Period6 = "Levy Contract",
                                    Period7 = "Levy Contract",
                                    Period8 = "Levy Contract",
                                    Period9 = "Levy Contract",
                                    Period10 = "Levy Contract",
                                    Period11 = "Levy Contract",
                                    Period12 = "Levy Contract"
                                }
                            }
                        }
                    },
                    PriceEpisodes = new EditableList<PriceEpisode>
                    {
                        new PriceEpisode
                        {
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Today,
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodeContractType = "Levy Contract",
                            },
                            PriceEpisodePeriodisedValues = new EditableList<PriceEpisodePeriodisedValues>
                            {
                                new PriceEpisodePeriodisedValues
                                {
                                    AttributeName = "PriceEpisodeOnProgPayment",
                                    Period1 = 10,
                                    Period2 = 20
                                }
                            }
                        }
                    }
                }
            };

            // act
            var events = builder.Build(learnerSubmission);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);
            events[0].Should().BeSameAs(expectedResult);
            events[0].ContractType.Should().Be(ContractType.Act1);

            mockMapper.Verify();
        }

        [Test]
        public void MixedContractTypeBuild()
        {
            // arrange
            var builder = new FunctionalSkillEarningEventBuilder(mapper);

            // act
            var events = builder.Build(learnerSubmission);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(2);

            var nonLevyContractTypeEarning = events.Single(x => x.ContractType == ContractType.Act2);
            nonLevyContractTypeEarning.Should().BeOfType<Act2FunctionalSkillEarningsEvent>();
            nonLevyContractTypeEarning.Should().NotBeNull();
            nonLevyContractTypeEarning.Earnings.Count.Should().Be(1);

            var act2Periods = nonLevyContractTypeEarning.Earnings.First().Periods.OrderBy(p => p.Period).ToList();
            act2Periods.Count.Should().Be(12);
            act2Periods.Take(4).Where(x => x.Amount != 0).Should().HaveCount(4);
            act2Periods.Skip(4).Where(x => x.Amount == 0).Should().HaveCount(8);

            var levyContractTypeEarning = events.Single(x => x.ContractType == ContractType.Act1);
            levyContractTypeEarning.Should().BeOfType<Act1FunctionalSkillEarningsEvent>();
            levyContractTypeEarning.Should().NotBeNull();
            levyContractTypeEarning.Earnings.Count.Should().Be(1);

            var act1Periods = levyContractTypeEarning.Earnings.First().Periods.OrderBy(p => p.Period).ToList();
            act1Periods.Count.Should().Be(12);
            act1Periods.Take(6).Where(x => x.Amount == 0).Should().HaveCount(6);
            act1Periods.Skip(6).Where(x => x.Amount != 0).Should().HaveCount(6);

            //mockMapper.Verify();
        }


        [Test]
        public void MapFundingLineTypeCorrectly()
        {
            // arrange
            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            
            // act
            var events = builder.Build(learnerSubmission);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(2);

            var nonLevyContractTypeEarning = events.Single(x => x.ContractType == ContractType.Act2);
            nonLevyContractTypeEarning.Should().BeOfType<Act2FunctionalSkillEarningsEvent>();
            nonLevyContractTypeEarning.LearningAim.Should().NotBeNull();
            nonLevyContractTypeEarning.LearningAim.FundingLineType.Should().Be("16-18 Apprenticeship Non-Levy Contract (procured)");

            var levyContractTypeEarning = events.Single(x => x.ContractType == ContractType.Act1);
            levyContractTypeEarning.Should().BeOfType<Act1FunctionalSkillEarningsEvent>();
            levyContractTypeEarning.LearningAim.Should().NotBeNull();
            levyContractTypeEarning.LearningAim.FundingLineType.Should().Be("16-18 Apprenticeship (Employer on App Service)");

            //mockMapper.Verify();
        }

        private static T GetFunctionalSkillEarningsEvent<T>() where T : FunctionalSkillEarningsEvent, new()
        {
            var earningPeriods = new List<EarningPeriod>
            {
                new EarningPeriod {Period = 1, Amount = 1},
                new EarningPeriod {Period = 2, Amount = 2},
                new EarningPeriod {Period = 3, Amount = 3},
                new EarningPeriod {Period = 4, Amount = 4},
                new EarningPeriod {Period = 5, Amount = 5},
                new EarningPeriod {Period = 6, Amount = 6},
                new EarningPeriod {Period = 7, Amount = 7},
                new EarningPeriod {Period = 8, Amount = 8},
                new EarningPeriod {Period = 9, Amount = 9},
                new EarningPeriod {Period = 10, Amount = 10},
                new EarningPeriod {Period = 11, Amount = 11},
                new EarningPeriod {Period = 12, Amount = 12},
            };

            var earnings = new List<FunctionalSkillEarning>
            {
                new FunctionalSkillEarning
                {
                    Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                    Periods = earningPeriods.AsReadOnly()
                },
            };

            return new T
            {
                Earnings = earnings.AsReadOnly()
            };
        }
    }
}
