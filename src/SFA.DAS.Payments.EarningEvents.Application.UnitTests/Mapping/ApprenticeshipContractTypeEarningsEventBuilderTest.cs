using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Extras.Moq;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.Core.Validation;
using SFA.DAS.Payments.EarningEvents.Application.Interfaces;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain;
using SFA.DAS.Payments.EarningEvents.Domain.Mapping;
using SFA.DAS.Payments.EarningEvents.Domain.Validation.Learner;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests.Mapping
{
    [TestFixture]
    public class ApprenticeshipContractTypeEarningsEventBuilderTest
    {

        private AutoMock mocker;
        private Mock<IApprenticeshipContractTypeEarningsEventFactory> factory;
        private IMapper mapper;
        private Mock<IConfigurationHelper> configurationHelper;
        private FM36Learner learner;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
        }


        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();

            configurationHelper = mocker.Mock<IConfigurationHelper>();

            configurationHelper
                .Setup(x => x.HasSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns(true);

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns("true");

            factory = mocker.Mock<IApprenticeshipContractTypeEarningsEventFactory>();

            mocker.Provide<IMapper>(mapper);

            learner = new FM36Learner
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
                            PriceEpisodeAimSeqNumber = 1,
                            EpisodeStartDate = new DateTime(2018, 8, 2),
                            EpisodeEffectiveTNPStartDate =new DateTime(2019, 7, 1),
                            PriceEpisodePlannedEndDate = DateTime.Today,
                            PriceEpisodeActualEndDate = DateTime.Today,
                            PriceEpisodePlannedInstalments = 12,
                            PriceEpisodeCompletionElement = 3000,
                            PriceEpisodeInstalmentValue = 1000,
                            TNP1 = 15000,
                            TNP2 = 15000,
                            PriceEpisodeCompleted = true,
                            PriceEpisodeCumulativePMRs = 13,
                            PriceEpisodeCompExemCode = 14,
                            PriceEpisodeTotalTNPPrice = 30000,
                            PriceEpisodeContractType = "Levy Contract",
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
            };

            var incentiveTypes = GetIncentiveTypes();

            foreach (var incentiveType in incentiveTypes)
            {
                learner.PriceEpisodes.First().PriceEpisodePeriodisedValues.Add(new PriceEpisodePeriodisedValues
                {
                    AttributeName = incentiveType,
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
                });
            }


        }

        [Test]
        [TestCase("true",10)]
        [TestCase("false", 0)]
        public void When_GenerateTransactionType4to16Payments_Is_False_No_IncentiveEarnings_Are_Generates(string generateTransactionType4To16PaymentsConfig, int expectedIncentiveCounts)
        {
            // arrange

            var actEarnings = new List<ApprenticeshipContractTypeEarningsEvent>
            {
               new ApprenticeshipContractType1EarningEvent(),
               new ApprenticeshipContractType2EarningEvent()
            };

            configurationHelper
                .Setup(x => x.GetSetting("Settings", "GenerateTransactionType4to16Payments"))
                .Returns(generateTransactionType4To16PaymentsConfig);

            factory
                .SetupSequence(x => x.Create(It.IsAny<string>()))
                .Returns(actEarnings[0])
                .Returns(actEarnings[1]);
            
            var learnerSubmission = new ProcessLearnerCommand
            {
                Learner = learner,
                CollectionYear = 1819,
                Ukprn = 12345,
                JobId = 69,
                CollectionPeriod = 1,
                IlrSubmissionDateTime = DateTime.UtcNow,
                SubmissionDate = DateTime.UtcNow
            };

            // act
            var result = mocker.Create<ApprenticeshipContractTypeEarningsEventBuilder>().Build(learnerSubmission);

            // assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expectedIncentiveCounts, result[0].IncentiveEarnings.Count);


        }

        private static IEnumerable<string> GetIncentiveTypes()
        {
            yield return "PriceEpisodeFirstEmp1618Pay";
            yield return "PriceEpisodeFirstProv1618Pay";
            yield return "PriceEpisodeSecondEmp1618Pay";
            yield return "PriceEpisodeSecondProv1618Pay";
            yield return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
            yield return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
            yield return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
            yield return "PriceEpisodeFirstDisadvantagePayment";
            yield return "PriceEpisodeSecondDisadvantagePayment";
            yield return "PriceEpisodeLSFCash";
        }

        private static string MapIncentiveType(IncentiveEarningType incentiveType)
        {
            switch (incentiveType)
            {
                case IncentiveEarningType.First16To18EmployerIncentive:
                    return "PriceEpisodeFirstEmp1618Pay";
                case IncentiveEarningType.First16To18ProviderIncentive:
                    return "PriceEpisodeFirstProv1618Pay";
                case IncentiveEarningType.Second16To18EmployerIncentive:
                    return "PriceEpisodeSecondEmp1618Pay";
                case IncentiveEarningType.Second16To18ProviderIncentive:
                    return "PriceEpisodeSecondProv1618Pay";
                case IncentiveEarningType.OnProgramme16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftOnProgPayment";
                case IncentiveEarningType.Completion16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftCompletionPayment";
                case IncentiveEarningType.Balancing16To18FrameworkUplift:
                    return "PriceEpisodeApplic1618FrameworkUpliftBalancing";
                case IncentiveEarningType.FirstDisadvantagePayment:
                    return "PriceEpisodeFirstDisadvantagePayment";
                case IncentiveEarningType.SecondDisadvantagePayment:
                    return "PriceEpisodeSecondDisadvantagePayment";
                case IncentiveEarningType.LearningSupport:
                    return "PriceEpisodeLSFCash";
                default:
                    return string.Empty;
            }
        }
    }
}
