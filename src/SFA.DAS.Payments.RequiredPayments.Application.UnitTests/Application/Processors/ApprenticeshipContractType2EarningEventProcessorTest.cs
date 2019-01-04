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
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class ApprenticeshipContractType2EarningEventProcessorTest
    {
        private AutoMock mocker;
        private ApprenticeshipContractType2EarningEventProcessor act2EarningEventProcessor;
        private Mock<IPaymentDueProcessor> paymentDueProcessorMock;
        private Mock<IRepositoryCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IApprenticeshipKeyService> apprenticeshipKeyServiceMock;
        private Mock<IPaymentHistoryRepository> paymentHistoryRepositoryMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<RequiredPaymentsProfile>();
            });
            Mapper.AssertConfigurationIsValid();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Mapper.Reset();
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            paymentDueProcessorMock = mocker.Mock<IPaymentDueProcessor>();
            paymentHistoryCacheMock = mocker.Mock<IRepositoryCache<PaymentHistoryEntity[]>>();
            apprenticeshipKeyServiceMock = mocker.Mock<IApprenticeshipKeyService>();
            paymentHistoryRepositoryMock = mocker.Mock<IPaymentHistoryRepository>();

            act2EarningEventProcessor = mocker.Create<ApprenticeshipContractType2EarningEventProcessor>(
                new NamedParameter("apprenticeshipKey", "key"), 
                new NamedParameter("mapper", Mapper.Instance));
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
            var period = new CalendarPeriod(2018, 9);

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new List<OnProgrammeEarning>()
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
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new[] { new PaymentHistoryEntity { CollectionPeriod = "1819-R02", DeliveryPeriod = "1819-R02"} };

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, period)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(1).Verifiable();

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
            var period = new CalendarPeriod(2018, 9);

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new List<OnProgrammeEarning>()
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
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, period)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(0).Verifiable();

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
            var deliveryPeriod = new CalendarPeriod(2018, 9);

            var earningEvent = new ApprenticeshipContractType2EarningEvent
            {
                Ukprn = 1,
                CollectionPeriod = new CalendarPeriod(2018, 10),
                CollectionYear = deliveryPeriod.AcademicYear,
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                OnProgrammeEarnings = new ReadOnlyCollection<OnProgrammeEarning>(new List<OnProgrammeEarning>()
                {
                    new OnProgrammeEarning
                    {
                        Type = OnProgrammeEarningType.Balancing,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>()
                        {
                            new EarningPeriod
                            {
                                Amount = amount,
                                Period = deliveryPeriod.Period,
                                PriceEpisodeIdentifier = priceEpisodeIdentifier
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new []
            {
                new PaymentHistoryEntity
                {
                    Amount = 100,
                    PriceEpisodeIdentifier = "2",
                    CollectionPeriod = new CalendarPeriod(2018, 9).Name,
                    DeliveryPeriod = new  CalendarPeriod(2018, 9).Name,
                    TransactionType = (int)IncentiveEarningType.Balancing16To18FrameworkUplift,
                    Ukprn = 1,
                    LearnAimReference = earningEvent.LearningAim.Reference,
                    LearnerReferenceNumber = earningEvent.Learner.ReferenceNumber,
                    ExternalId = Guid.NewGuid()
                }
            };

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey(earningEvent.LearningAim.Reference, (int)OnProgrammeEarningType.Balancing, deliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(amount, It.IsAny<Payment[]>())).Returns(-100).Verifiable();

            // act
            var actualRequiredPayment = await act2EarningEventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual("2", actualRequiredPayment.First().PriceEpisodeIdentifier);

        }

        private static LearningAim CreateLearningAim()
        {
            return new LearningAim
            {
                ProgrammeType = 5,
                PathwayCode = 6,
                StandardCode = 7,
                FrameworkCode = 8,
                Reference = "9",
                FundingLineType = "11"
            };
        }

        private static Learner CreateLearner()
        {
            return new Learner
            {
                ReferenceNumber = "3",
                Uln = 4
            };
        }
    }
}