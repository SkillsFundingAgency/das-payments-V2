using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application.Handlers;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Handlers
{
    [TestFixture]
    public class ApprenticeshipContractType2PaymentDueEventHandlerTest
    {
        private AutoMock mocker;
        private IApprenticeshipContractType2PaymentDueEventHandler act2PaymentDueEventHandler;
        private Mock<IApprenticeshipContractType2PaymentDueProcessor> paymentDueProcessorMock;
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
            paymentDueProcessorMock = mocker.Mock<IApprenticeshipContractType2PaymentDueProcessor>();
            paymentHistoryCacheMock = mocker.Mock<IRepositoryCache<PaymentHistoryEntity[]>>();
            apprenticeshipKeyServiceMock = mocker.Mock<IApprenticeshipKeyService>();
            paymentHistoryRepositoryMock = mocker.Mock<IPaymentHistoryRepository>();

            act2PaymentDueEventHandler =
                mocker.Create<ApprenticeshipContractType2PaymentDueEventHandler>(new Parameter[]
                    {new NamedParameter("apprenticeshipKey", "key"),});
            //new ApprenticeshipContractType2PaymentDueEventHandler(
            //    paymentDueProcessorMock.Object,
            //    paymentHistoryCacheMock.Object,
            //    Mapper.Instance,
            //    apprenticeshipKeyServiceMock.Object,
            //    paymentHistoryRepositoryMock.Object,
            //    "key"
            //);
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
                await act2PaymentDueEventHandler.HandlePaymentDue(null);
            }
            catch (ArgumentNullException ex)
            {
                Assert.AreEqual("paymentDue", ex.ParamName);
                return;
            }

            Assert.Fail();
        }

        [Test]
        public async Task TestHandleNormalEvent()
        {
            // arrange
            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                Ukprn = 1,
                PriceEpisodeIdentifier = "2",
                AmountDue = 100,
                CollectionPeriod = new CalendarPeriod(2018, 9),
                DeliveryPeriod = new CalendarPeriod(2018, 9),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Learning
            };

            var paymentHistoryEntities = new[] { new PaymentHistoryEntity() }; 

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(1).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(1, actualRequiredPayment.AmountDue);
            Assert.AreEqual(paymentDue.LearningAim.Reference, actualRequiredPayment.LearningAim.Reference);
        }

        [Test]
        public async Task TestNoEventProducedWhenZeroToPay()
        {
            // arrange
            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                Ukprn = 1,
                PriceEpisodeIdentifier = "2",
                AmountDue = 100,
                CollectionPeriod = new CalendarPeriod(2018, 9),
                DeliveryPeriod = new CalendarPeriod(2018, 9),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Learning
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(0).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue);

            // assert
            Assert.IsNull(actualRequiredPayment);
        }

        [Test]
        public async Task TestPopulateHistory()
        {
            // arrange
            var paymentHistory = new List<PaymentHistoryEntity>
            {
                new PaymentHistoryEntity
                {
                    PriceEpisodeIdentifier = "1",
                    LearnAimReference = "1",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R01"
                },
                new PaymentHistoryEntity
                {
                    PriceEpisodeIdentifier = "2",
                    LearnAimReference = "2",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R02"
                },
                new PaymentHistoryEntity
                {
                    PriceEpisodeIdentifier = "3",
                    LearnAimReference = "3",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R01"
                }
            };

            paymentHistoryRepositoryMock.Setup(r => r.GetPaymentHistory(It.IsAny<ApprenticeshipKey>(), It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("1", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("2", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("3", 3, It.IsAny<CalendarPeriod>())).Returns("2").Verifiable();

            paymentHistoryCacheMock.Setup(c => c.Contains("1", It.IsAny<CancellationToken>())).ReturnsAsync(false).Verifiable();
            paymentHistoryCacheMock.Setup(c => c.Contains("2", It.IsAny<CancellationToken>())).ReturnsAsync(true).Verifiable();
            paymentHistoryCacheMock.Setup(c => c.Clear("2", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
            paymentHistoryCacheMock.Setup(c => c.Add("1", It.Is<PaymentHistoryEntity[]>(p => p.Length == 2 && p[0].PriceEpisodeIdentifier == "1" && p[1].PriceEpisodeIdentifier == "2"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Verifiable();
            paymentHistoryCacheMock.Setup(c => c.Add("2", It.Is<PaymentHistoryEntity[]>(p => p.Length == 1 && p[0].PriceEpisodeIdentifier == "3"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Verifiable();


            // act
            await act2PaymentDueEventHandler.PopulatePaymentHistoryCache(CancellationToken.None).ConfigureAwait(false);

            // assert
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
