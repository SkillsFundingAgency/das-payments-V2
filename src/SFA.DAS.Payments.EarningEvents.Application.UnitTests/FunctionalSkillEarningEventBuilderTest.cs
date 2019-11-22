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
                            LearningDeliveryValues = new LearningDeliveryValues { LearnAimRef = "ZPROG001" },
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
            var learnerSubmissionModel = new ProcessLearnerCommand
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
            var events = builder.Build(learnerSubmissionModel);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);

        }

        [Test]
        public void MultipleLearningDeliveriesMapCorrectContractType()
        {
            // arrange
            var learnerSubmission2 = new ProcessLearnerCommand
            {
                CollectionPeriod = 1,
                CollectionYear = 1920,
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                        new LearningDelivery
                        {
                            AimSeqNumber = 1,
                            LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "English"},
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
                            AimSeqNumber = 2,
                            LearningDeliveryValues = new LearningDeliveryValues {LearnAimRef = "Maths"},
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
                                    Period1 = "Non-Levy Contract",
                                    Period2 = "Non-Levy Contract",
                                    Period3 = "Non-Levy Contract",
                                    Period4 = "Non-Levy Contract",
                                    Period5 = "Non-Levy Contract",
                                    Period6 = "Non-Levy Contract",
                                    Period7 = "Non-Levy Contract",
                                    Period8 = "Non-Levy Contract",
                                    Period9 = "Non-Levy Contract",
                                    Period10 = "Non-Levy Contract",
                                    Period11 = "Non-Levy Contract",
                                    Period12 = "Non-Levy Contract"
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
                    },
                    PriceEpisodes = new EditableList<PriceEpisode> { }
                }
            };

            var sut = new FunctionalSkillEarningEventBuilder(mapper);

            // act
            var events = sut.Build(learnerSubmission2);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(2);
            events[0].Should().BeAssignableTo<Act1FunctionalSkillEarningsEvent>();
            events[0].ContractType.Should().Be(ContractType.Act1);
            events[1].Should().BeAssignableTo<Act2FunctionalSkillEarningsEvent>();
            events[1].ContractType.Should().Be(ContractType.Act2);
        }


        [Test]
        public void FunctionalSkillBuild()
        {
            // arrange
            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            var learnerSubmissionModel = new ProcessLearnerCommand
            {
                CollectionPeriod = 1,
                CollectionYear = 1920,
                Learner = new FM36Learner
                {
                    LearningDeliveries = new EditableList<LearningDelivery>
                    {
                           new LearningDelivery
                        {
                            AimSeqNumber = 1,
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
                            AimSeqNumber = 2,
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
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName= "FundLineType",
                                    Period1 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period2 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period3 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period4 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period5 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period6 ="16-18 Apprenticeship (Employer on App Service)",
                                    Period7 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period8 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period9 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period10 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period11 = "16-18 Apprenticeship (Employer on App Service)",
                                    Period12 = "16-18 Apprenticeship (Employer on App Service)"
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
            var events = builder.Build(learnerSubmissionModel);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);
            events[0].ContractType.Should().Be(ContractType.Act1);
            events[0].Earnings[0].Periods.Should().HaveCount(12);
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
        }

        [Test]
        public void IncludesLearningSupport()
        {
            // arrange
            var processLearnerCommand = new ProcessLearnerCommand
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
                            PriceEpisodeIdentifier = "20-593-1-01/08/2019",
                            PriceEpisodeValues = new PriceEpisodeValues
                            {
                                EpisodeStartDate = DateTime.Parse("2019-08-01T00:00:00+00:00"),
                                PriceEpisodeActualEndDate = DateTime.Parse("2019-10-06T00:00:00+00:00"),
                                PriceEpisodeFundLineType = "19+ Apprenticeship (Employer on App Service)",
                                EpisodeEffectiveTNPStartDate = DateTime.Parse("2017-05-08T00:00:00+00:00"),
                                PriceEpisodeContractType = "Contract for services with the employer",
                                PriceEpisodeAimSeqNumber = 1,
                                PriceEpisodePlannedEndDate = DateTime.Parse("2019-10-14T00:00:00+00:00"),
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
                                    Period3 = 3000,
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
                                    Period3 = 0,
                                    Period4 = 0,
                                    Period5 = 0,
                                    Period6 = 0,
                                    Period7 = 0,
                                    Period8 = 0,
                                    Period9 = 0,
                                    Period10 = 0,
                                    Period11 = 0,
                                    Period12 = 0
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
                                LearnStartDate = DateTime.Parse("2019-08-06T00:00:00+00:00"),
                                LearnDelMathEng = true
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
                                },
                                new LearningDeliveryPeriodisedValues
                                {
                                    AttributeName = "LearnSuppFundCash",
                                    Period1 = 0,
                                    Period2 = 0,
                                    Period3 = 150,
                                    Period4 = 150,
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
                            LearningDeliveryPeriodisedTextValues = new List<LearningDeliveryPeriodisedTextValues>
                            {
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "LearnDelContType",
                                    Period1 = "Contract for services with the employer",
                                    Period2 = "Contract for services with the employer",
                                    Period3 = "Contract for services with the employer",
                                    Period4 = "Contract for services with the employer",
                                    Period5 = "Contract for services with the employer",
                                    Period6 = "Contract for services with the employer",
                                    Period7 = "Contract for services with the employer",
                                    Period8 = "Contract for services with the employer",
                                    Period9 = "Contract for services with the employer",
                                    Period10 = "Contract for services with the employer",
                                    Period11 = "Contract for services with the employer",
                                    Period12 = "Contract for services with the employer"
                                },
                                new LearningDeliveryPeriodisedTextValues
                                {
                                    AttributeName = "FundLineType",
                                    Period1 = "19+ Apprenticeship (Employer on App Service)",
                                    Period2 = "19+ Apprenticeship (Employer on App Service)",
                                    Period3 = "19+ Apprenticeship (Employer on App Service)",
                                    Period4 = "19+ Apprenticeship (Employer on App Service)",
                                    Period5 = "19+ Apprenticeship (Employer on App Service)",
                                    Period6 = "19+ Apprenticeship (Employer on App Service)",
                                    Period7 = "19+ Apprenticeship (Employer on App Service)",
                                    Period8 = "19+ Apprenticeship (Employer on App Service)",
                                    Period9 = "19+ Apprenticeship (Employer on App Service)",
                                    Period10 = "19+ Apprenticeship (Employer on App Service)",
                                    Period11 = "19+ Apprenticeship (Employer on App Service)",
                                    Period12 = "19+ Apprenticeship (Employer on App Service)"
                                }
    }
                        },
                    }
                }
            };

            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            
            // act
            var events = builder.Build(processLearnerCommand);

            // assert
            events.Should().NotBeNull();
            events.Should().HaveCount(1);
            events.Single().Earnings.Should().HaveCount(2);
            events.Single().Earnings.Single(x => x.Type == FunctionalSkillType.LearningSupport).Periods.Should().HaveCount(12);
        }
    }
}
