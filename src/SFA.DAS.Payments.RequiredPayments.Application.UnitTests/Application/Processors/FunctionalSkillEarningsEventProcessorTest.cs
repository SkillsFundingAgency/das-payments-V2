using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
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
        private Mock<IDataCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IRequiredPaymentService> requiredPaymentsService;
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
            paymentHistoryCacheMock = mocker.Mock<IDataCache<PaymentHistoryEntity[]>>();
            requiredPaymentsService = mocker.Mock<IRequiredPaymentService>();
            
            eventProcessor = mocker.Create<FunctionalSkillEarningsEventProcessor>(
                new NamedParameter("apprenticeshipKey", "key"), 
                new NamedParameter("mapper", mapper),
                new AutowiringParameter());
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
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
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            byte deliveryPeriod = 2;

            var earningEvent = new FunctionalSkillEarningsEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
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
                                Period = deliveryPeriod,
                                Amount = 100,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.9m,
                            }
                        })
                    }
                })
            };

            var requiredPayments = new List<RequiredPayment>
            {
                new RequiredPayment
                {
                    Amount = 100,
                    EarningType = EarningType.Incentive,
                    SfaContributionPercentage = 0.9m,
                },
            };

            var paymentHistoryEntities = new[] {new PaymentHistoryEntity {CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2), DeliveryPeriod = 2}};

            mocker.Mock<IPaymentKeyService>()
                .Setup(s => s.GeneratePaymentKey("9", (int)FunctionalSkillType.BalancingMathsAndEnglish, It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities))
                .Verifiable();
            requiredPaymentsService.Setup(p => p.GetRequiredPayments(It.IsAny<Earning>(), It.IsAny<List<Payment>>()))
                .Returns(requiredPayments)
                .Verifiable();

            // act
            var actualRequiredPayment = await eventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.Count);
            Assert.AreEqual(100, actualRequiredPayment.First().AmountDue);
            Assert.AreEqual(earningEvent.LearningAim.Reference, actualRequiredPayment.First().LearningAim.Reference);
            Assert.AreEqual("2", actualRequiredPayment.First().PriceEpisodeIdentifier);
        }

        [Test]
        public async Task TestNoEventProducedWhenZeroToPay()
        {
            // arrange
            var period = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(1819, 2);
            
            var earningEvent = new FunctionalSkillEarningsEvent
            {
                Ukprn = 1,
                CollectionPeriod = period,
                CollectionYear = period.AcademicYear,
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
                                Period = 2,
                                Amount = 100,
                                PriceEpisodeIdentifier = "2",
                                SfaContributionPercentage = 0.8m,
                            }
                        })
                    }
                })
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>()
                .Setup(s => s.GeneratePaymentKey("9", (int)FunctionalSkillType.OnProgrammeMathsAndEnglish, It.IsAny<short>(), It.IsAny<byte>()))
                .Returns("payment key")
                .Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            requiredPaymentsService.Setup(p => p.GetRequiredPayments(It.IsAny<Earning>(), It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>())
                .Verifiable();

            // act
            var actualRequiredPayment = await eventProcessor.HandleEarningEvent(earningEvent, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.AreEqual(0, actualRequiredPayment.Count);
        }
    }
}
