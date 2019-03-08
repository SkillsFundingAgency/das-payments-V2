using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningEventProcessorTest
    {
        private AutoMock mocker;
        private ApprenticeshipContractType2EarningEventProcessor act2EarningEventProcessor;
        private Mock<IPaymentDueProcessor> paymentDueProcessorMock;
        private Mock<IDataCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IApprenticeshipKeyService> apprenticeshipKeyServiceMock;
        private Mock<IPaymentHistoryRepository> paymentHistoryRepositoryMock;
        private Mapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetStrict();
            paymentDueProcessorMock = mocker.Mock<IPaymentDueProcessor>();
            paymentHistoryCacheMock = mocker.Mock<IDataCache<PaymentHistoryEntity[]>>();
            apprenticeshipKeyServiceMock = mocker.Mock<IApprenticeshipKeyService>();
            paymentHistoryRepositoryMock = mocker.Mock<IPaymentHistoryRepository>();

            act2EarningEventProcessor = mocker.Create<ApprenticeshipContractType2EarningEventProcessor>(
                new NamedParameter("apprenticeshipKey", "key"), 
                new NamedParameter("mapper", mapper));
        }

        [TearDown]
        public void TearDown()
        {
            paymentDueProcessorMock.Verify();
            paymentHistoryCacheMock.Verify();
            apprenticeshipKeyServiceMock.Verify();
            paymentHistoryRepositoryMock.Verify();
        }

        [Test]
        public async Task TestHandleNullEvent()
        {
            // arrange            
            // act
            // assert
            try
            {
                await act2EarningEventProcessor.HandleEarningEvent(null, paymentHistoryCacheMock.Object, CancellationToken.None);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("earningEvent", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task TestHandleNormalEvent()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = "2"
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new[] { new PaymentHistoryEntity
            {
                CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                DeliveryPeriod = 2,
            } };

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).Returns(Task.FromResult(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities))).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<IEnumerable<Payment>>())).Returns(1).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateSfaContributionPercentage(0, 100, It.IsAny<Payment[]>())).Returns(100).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(1, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
        }

        [Test]
        public async Task TestNoEventProducedWhenZeroToPay()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = "2"
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<IEnumerable<Payment>>())).Returns(0).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.AreEqual(0, actualRequiredPayment.Count);
        }

        [Test]
        [TestCase(0, "1")]
        [TestCase(0, null)]
        [TestCase(50, "1")]
        public async Task TestPriceEpisodeIdentifierPickedFromHistoryForRefunds(decimal amount, string priceEpisodeIdentifier)
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 3);
            var deliveryPeriod = (byte)2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = amount,
                                Period = deliveryPeriod,
                                PriceEpisodeIdentifier = priceEpisodeIdentifier
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new []
            {
                new PaymentHistoryEntity
                {
                    Amount = 100,
                    PriceEpisodeIdentifier = "2",
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2),
                    DeliveryPeriod = 2,
                    TransactionType = (int)IncentiveEarningType.Balancing16To18FrameworkUplift,
                    Ukprn = 1,
                    LearnAimReference = earningEvent.LearningAim.Reference,
                    LearnerReferenceNumber = earningEvent.Learner.ReferenceNumber,
                    ExternalId = Guid.NewGuid()
                }
            };

            mocker.Mock<IPaymentKeyService>()
                .Setup(s => s.GeneratePaymentKey(earningEvent.LearningAim.Reference, (int)OnProgrammeEarningType.Balancing,
                    It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(amount, It.IsAny<IEnumerable<Payment>>())).Returns(-100).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateSfaContributionPercentage(0, amount, It.IsAny<Payment[]>())).Returns(100).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual("2", actualRequiredPayment.First().PriceEpisodeIdentifier);

        }

        [Test]
        public async Task TestSfaContributionIsCalculatedForZeroEarningRefunds()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 3);
            byte deliveryPeriod = 2;

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                SfaContributionPercentage = 0,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = deliveryPeriod,
                                Amount = 0,
                                PriceEpisodeIdentifier = "priceEpisodeIdentifier",
                                SfaContributionPercentage = 0
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new List<PaymentHistoryEntity>();

            mocker.Mock<IPaymentKeyService>()
                .Setup(s => s.GeneratePaymentKey(earningEvent.LearningAim.Reference, (int)OnProgrammeEarningType.Balancing,
                    It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities.ToArray())).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(0, It.IsAny<IEnumerable<Payment>>())).Returns(-100).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateSfaContributionPercentage(0, 0, It.IsAny<Payment[]>())).Returns(.77m).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotEmpty(actualRequiredPayment);
            Assert.AreEqual(.77m, ((ApprenticeshipContractType2RequiredPaymentEvent)actualRequiredPayment[0]).SfaContributionPercentage);
        }

        [Test]
        public async Task TestFuturePeriodsCutOff()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            
            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                OnProgrammeEarnings = new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Learning,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = 100,
                                Period = period.Period,
                                PriceEpisodeIdentifier = "2"
                            },
                            new EarningPeriod
                            {
                                Amount = 200,
                                Period = (byte)(period.Period + 1),
                                PriceEpisodeIdentifier = "2"
                            }
                        })
                    }
                }
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1,It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<IEnumerable<Payment>>())).Returns(100).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateSfaContributionPercentage(0, 100, It.IsAny<Payment[]>())).Returns(100).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(100, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
        }
    }
}