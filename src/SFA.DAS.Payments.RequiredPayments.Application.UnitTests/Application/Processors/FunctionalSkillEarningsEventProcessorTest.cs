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
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class FunctionalSkillEarningsEventProcessorTest
    {
        private AutoMock mocker;
        private FunctionalSkillEarningsEventProcessor eventProcessor;
        private Mock<IPaymentDueProcessor> paymentDueProcessorMock;
        private Mock<IRepositoryCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
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
            mocker = AutoMock.GetLoose();
            paymentDueProcessorMock = mocker.Mock<IPaymentDueProcessor>();
            paymentHistoryCacheMock = mocker.Mock<IRepositoryCache<PaymentHistoryEntity[]>>();
            apprenticeshipKeyServiceMock = mocker.Mock<IApprenticeshipKeyService>();
            paymentHistoryRepositoryMock = mocker.Mock<IPaymentHistoryRepository>();


            eventProcessor = mocker.Create<FunctionalSkillEarningsEventProcessor>(new NamedParameter("apprenticeshipKey", "key"), new NamedParameter("mapper", mapper));
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
                await eventProcessor.HandleEarningEvent(null, null, CancellationToken.None);
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
            var deliveryPeriod = new CalendarPeriod(2018, 9);

            var earningEvent = new FunctionalSkillEarningsEvent
            {
                Ukprn = 1,
                CollectionPeriod = new CalendarPeriod(2018, 9),
                CollectionYear = deliveryPeriod.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.BalancingMathsAndEnglish,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = deliveryPeriod.Period,
                                Amount = 100,
                                PriceEpisodeIdentifier = "2"
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new[] {new PaymentHistoryEntity {CollectionPeriod = "1819-R02", DeliveryPeriod = "1819-R02"}};

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", (int)FunctionalSkillType.BalancingMathsAndEnglish, deliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(1).Verifiable();

            // act
            var actualRequiredPayment = await eventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(1, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
            Assert.AreEqual("2", actualRequiredPayment.First().PriceEpisodeIdentifier);
        }

        [Test]
        public async Task TestNoEventProducedWhenZeroToPay()
        {
            // arrange
            var deliveryPeriod = new CalendarPeriod(2018, 9);

            var earningEvent = new FunctionalSkillEarningsEvent
            {
                Ukprn = 1,
                CollectionPeriod = new CalendarPeriod(2018, 9),
                CollectionYear = deliveryPeriod.AcademicYear,
                Learner = EarningEventDataHelper.CreateLearner(),
                LearningAim = EarningEventDataHelper.CreateLearningAim(),
                Earnings = new ReadOnlyCollection<FunctionalSkillEarning>(new List<FunctionalSkillEarning>
                {
                    new FunctionalSkillEarning
                    {
                        Type = FunctionalSkillType.OnProgrammeMathsAndEnglish,
                        Periods = new ReadOnlyCollection<EarningPeriod>(new List<EarningPeriod>
                        {
                            new EarningPeriod
                            {
                                Period = deliveryPeriod.Period,
                                Amount = 100,
                                PriceEpisodeIdentifier = "2"
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", (int)FunctionalSkillType.OnProgrammeMathsAndEnglish, deliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(0).Verifiable();

            // act
            var actualRequiredPayment = await eventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.AreEqual(0, actualRequiredPayment.Count);
        }
    }
}
