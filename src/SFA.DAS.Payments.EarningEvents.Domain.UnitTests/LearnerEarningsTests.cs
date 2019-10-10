using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using Castle.Components.DictionaryAdapter;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;
using FluentAssertions;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using AutoMapper;
using System;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class LearnerEarningsTests
    {
        private AutoMock mocker;
        private FM36Learner learner;
        private Mock<ILearnerValidator> validatorMock;
        private Mock<IApprenticeshipContractTypeEarningsEventBuilder> actBuilder;
        private Mock<IFunctionalSkillEarningsEventBuilder> functionalSkillBuilder;
        private Mock<IConfigurationHelper> configurationHelper;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            validatorMock = mocker.Mock<ILearnerValidator>();
            actBuilder = mocker.Mock<IApprenticeshipContractTypeEarningsEventBuilder>();
            functionalSkillBuilder = mocker.Mock<IFunctionalSkillEarningsEventBuilder>();
            validatorMock = mocker.Mock<ILearnerValidator>();
            configurationHelper = mocker.Mock<IConfigurationHelper>();

            configurationHelper
                .Setup(x => x.HasSetting("Settings", "DoNotGenerateACT1TransactionType4To16Payments"))
                .Returns(true);

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "DoNotGenerateACT1TransactionType4To16Payments"))
                .Returns("false");

            configurationHelper
                .Setup(x => x.HasSetting("Settings", "DoNotGenerateACT2Payments"))
                .Returns(true);

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "DoNotGenerateACT2Payments"))
                .Returns("false");

            validatorMock.Setup(x => x.Validate(It.IsAny<FM36Learner>())).Returns(new ValidationResult(new List<ValidationRuleResult>()));
            learner = new FM36Learner();
        }
        
        [Test]
        public void Fails_If_Validation_Fails()
        {
            mocker.Mock<ILearnerValidator>()
                .Setup(x => x.Validate(It.IsAny<FM36Learner>()))
                .Returns(new ValidationResult(new List<ValidationRuleResult> { ValidationRuleResult.Failure("some failure") }))
                .Verifiable();

            var learnerSubmission =new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);
            Assert.IsTrue(result.Validation.Failed);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void Generates_Apprenticeship_Contract_Earnings()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType2EarningEvent(),
                new ApprenticeshipContractType2EarningEvent()
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act2FunctionalSkillEarningsEvent(),
                new Act1FunctionalSkillEarningsEvent()
            };

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);


            // assert
            Assert.IsFalse(result.Validation.Failed);
            Assert.AreEqual(4, result.EarningEvents.Count);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void When_DoNotGenerateACT1TransactionType4To16Payments_Is_True_Payments_Are_Generated_Correctly()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType1EarningEvent
                {
                    OnProgrammeEarnings = new List<OnProgrammeEarning>
                    {
                        new OnProgrammeEarning()
                    },
                    IncentiveEarnings = new List<IncentiveEarning>
                    {
                        new IncentiveEarning(),
                        new IncentiveEarning()
                    }
                },
                new ApprenticeshipContractType2EarningEvent(),
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act2FunctionalSkillEarningsEvent(),
                new Act1FunctionalSkillEarningsEvent()
            };

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "DoNotGenerateACT1TransactionType4To16Payments"))
                .Returns("true");

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);
            
            // assert
            result.EarningEvents.Should().HaveCount(3);


            var act1Payments = result.EarningEvents.OfType<ApprenticeshipContractType1EarningEvent>().ToList();
            act1Payments.Should().HaveCount(1);
            act1Payments.First().IncentiveEarnings.Should().BeEmpty();

            result.EarningEvents.OfType<Act1FunctionalSkillEarningsEvent>().Should().BeEmpty();
            
        }

        [Test]
        public void When_DoNotGenerateACT2Payments_Is_True_Payments_Are_Generated_Correctly()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType1EarningEvent
                {
                    OnProgrammeEarnings = new List<OnProgrammeEarning>
                    {
                        new OnProgrammeEarning()
                    },
                    IncentiveEarnings = new List<IncentiveEarning>
                    {
                        new IncentiveEarning(),
                        new IncentiveEarning()
                    }
                },
                new ApprenticeshipContractType2EarningEvent(),
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act2FunctionalSkillEarningsEvent(),
                new Act1FunctionalSkillEarningsEvent()
            };

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "DoNotGenerateACT2Payments"))
                .Returns("true");

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);

            // assert
            result.EarningEvents.Should().HaveCount(2);


            var act1Payments = result.EarningEvents.OfType<ApprenticeshipContractType1EarningEvent>().ToList();
            act1Payments.Should().HaveCount(1);
            act1Payments.First().IncentiveEarnings.Should().HaveCount(2);

            var functionalSkillAct1Payments = result.EarningEvents.OfType<Act1FunctionalSkillEarningsEvent>().ToList();
            functionalSkillAct1Payments.Should().HaveCount(1);

            result.EarningEvents.OfType<ApprenticeshipContractType2EarningEvent>().Should().BeEmpty();
            result.EarningEvents.OfType<Act2FunctionalSkillEarningsEvent>().Should().BeEmpty();

        }

        [Test]
        public void RemovesFuturePeriodsForAct2()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType2EarningEvent
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    IncentiveEarnings = new List<IncentiveEarning>
                    {
                        new IncentiveEarning
                        {
                            Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        },
                        new IncentiveEarning
                        {
                            Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 3},
                                new EarningPeriod {Period = 4}
                            }.AsReadOnly()
                        }
                    },
                    OnProgrammeEarnings = new EditableList<OnProgrammeEarning>
                    {
                        new OnProgrammeEarning
                        {
                            Periods = new List<EarningPeriod>()
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        },
                        new OnProgrammeEarning
                        {
                            Periods = new List<EarningPeriod>()
                            {
                                new EarningPeriod {Period = 3},
                                new EarningPeriod {Period = 4}
                            }.AsReadOnly()
                        }
                    }
                }
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act2FunctionalSkillEarningsEvent
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    Earnings = new List<FunctionalSkillEarning>
                    {
                        new FunctionalSkillEarning
                        {
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        }
                    }.AsReadOnly()
                }
            };

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);

            // assert
            Assert.AreEqual(2, result.EarningEvents.Count);
            Assert.IsInstanceOf<ApprenticeshipContractTypeEarningsEvent>(result.EarningEvents[0]);
            var act2Earning = (ApprenticeshipContractTypeEarningsEvent)result.EarningEvents[0];

            Assert.AreEqual(1, act2Earning.IncentiveEarnings.Count);
            Assert.AreEqual(1, act2Earning.IncentiveEarnings[0].Periods.Count);
            Assert.AreEqual(1, act2Earning.IncentiveEarnings[0].Periods[0].Period);

            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings.Count);
            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings[0].Periods.Count);
            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings[0].Periods[0].Period);

            Assert.IsInstanceOf<Act2FunctionalSkillEarningsEvent>(result.EarningEvents[1]);
            var mathsEarning = (Act2FunctionalSkillEarningsEvent)result.EarningEvents[1];
            Assert.AreEqual(1, mathsEarning.Earnings.Count);
            Assert.AreEqual(1, mathsEarning.Earnings[0].Periods.Count);
            Assert.AreEqual(1, mathsEarning.Earnings[0].Periods[0].Period);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);

        }

        [Test]
        public void RemovesFuturePeriodsForAct1()
        {
            // arrange
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
                new ApprenticeshipContractType1EarningEvent
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    IncentiveEarnings = new List<IncentiveEarning>
                    {
                        new IncentiveEarning
                        {
                            Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        },
                        new IncentiveEarning
                        {
                            Type = IncentiveEarningType.Balancing16To18FrameworkUplift, Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 3},
                                new EarningPeriod {Period = 4}
                            }.AsReadOnly()
                        }
                    },
                    OnProgrammeEarnings = new EditableList<OnProgrammeEarning>
                    {
                        new OnProgrammeEarning
                        {
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        },
                        new OnProgrammeEarning
                        {
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 3},
                                new EarningPeriod {Period = 4}
                            }.AsReadOnly()
                        }
                    }
                }
            };

            var functionalSkillEarnings = new List<FunctionalSkillEarningsEvent>
            {
                new Act1FunctionalSkillEarningsEvent
                {
                    CollectionPeriod = new CollectionPeriod {AcademicYear = 1, Period = 1},
                    Earnings = new List<FunctionalSkillEarning>
                    {
                        new FunctionalSkillEarning
                        {
                            Periods = new List<EarningPeriod>
                            {
                                new EarningPeriod {Period = 1},
                                new EarningPeriod {Period = 2}
                            }.AsReadOnly()
                        }
                    }.AsReadOnly()
                }
            };

            actBuilder.Setup(b => b.Build(learnerSubmission)).Returns(actEarnings).Verifiable();
            functionalSkillBuilder.Setup(b => b.Build(learnerSubmission)).Returns(functionalSkillEarnings).Verifiable();

            // act
            var result = mocker.Create<LearnerSubmissionProcessor>().GenerateEarnings(learnerSubmission);

            // assert
            Assert.AreEqual(2, result.EarningEvents.Count);
            Assert.IsInstanceOf<ApprenticeshipContractTypeEarningsEvent>(result.EarningEvents[0]);
            var act2Earning = (ApprenticeshipContractTypeEarningsEvent)result.EarningEvents[0];

            Assert.AreEqual(1, act2Earning.IncentiveEarnings.Count);
            Assert.AreEqual(1, act2Earning.IncentiveEarnings[0].Periods.Count);
            Assert.AreEqual(1, act2Earning.IncentiveEarnings[0].Periods[0].Period);

            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings.Count);
            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings[0].Periods.Count);
            Assert.AreEqual(1, act2Earning.OnProgrammeEarnings[0].Periods[0].Period);

            Assert.IsInstanceOf<FunctionalSkillEarningsEvent>(result.EarningEvents[1]);
            var mathsEarning = (FunctionalSkillEarningsEvent)result.EarningEvents[1];
            Assert.AreEqual(1, mathsEarning.Earnings.Count);
            Assert.AreEqual(1, mathsEarning.Earnings[0].Periods.Count);
            Assert.AreEqual(1, mathsEarning.Earnings[0].Periods[0].Period);

            Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }

        [Test]
        public void Generate_Apprenticeship_Contract_Earnings_Ignoring_Invalid_Contracts()
        {
            // Arrange
            var sut = new ApprenticeshipContractTypeEarningsEventBuilder(
                new ApprenticeshipContractTypeEarningsEventFactory(),
                new Mapper(new MapperConfiguration(c => { })));

            learner = new FM36Learner
            {
                ULN = 12,
                LearnRefNumber = "12",
                PriceEpisodes = new List<ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode>
                {
                    new ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output.PriceEpisode
                    {
                        PriceEpisodeValues = new PriceEpisodeValues
                        {
                            EpisodeStartDate = DateTime.UtcNow,
                            PriceEpisodeContractType = "",
                            PriceEpisodeAimSeqNumber = 1,
                        },
                    },
                },
                LearningDeliveries = new List<LearningDelivery>
                {
                    new LearningDelivery
                    {
                        AimSeqNumber = 1,
                        LearningDeliveryValues = new LearningDeliveryValues
                        {
                            LearnAimRef = "ZPROG001",
                        },
                    }
                },
                HistoricEarningOutputValues = new List<HistoricEarningOutputValues> { },
            };

            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1920,
                CollectionPeriod = 1,
                Ukprn = 12345,
                JobId = 1
            };

            // Act
            var earningsEvents = sut.Build(learnerSubmission);


            // Assert
            //Assert.IsFalse(result.Validation.Failed);
            //Assert.AreEqual(4, result.EarningEvents.Count);

            //Mock.Verify(validatorMock, actBuilder, functionalSkillBuilder);
        }
    }
}