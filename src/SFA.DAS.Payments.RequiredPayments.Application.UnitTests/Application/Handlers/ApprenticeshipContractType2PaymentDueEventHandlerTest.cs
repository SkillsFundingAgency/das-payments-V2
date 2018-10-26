using System;
using System.Threading;
using System.Threading.Tasks;
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
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Handlers
{
    [TestFixture]
    public class ApprenticeshipContractType2PaymentDueEventHandlerTest
    {
        private IApprenticeshipContractType2PaymentDueEventHandler act2PaymentDueEventHandler;
        private Mock<IApprenticeshipContractType2PaymentDueProcessor> paymentDueProcessorMock;
        private Mock<IRepositoryCache<PaymentHistoryEntity[]>> paymentHistoryCacheMock;
        private Mock<IApprenticeshipKeyService> apprenticeshipKeyServiceMock;
        private Mock<IPaymentHistoryRepository> paymentHistoryRepositoryMock;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Mapper.Initialize(cfg => AutoMapperConfigurationFactory.CreateMappingConfig());
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Mapper.Reset();
        }

        [SetUp]
        public void SetUp()
        {
            paymentDueProcessorMock = new Mock<IApprenticeshipContractType2PaymentDueProcessor>(MockBehavior.Strict);
            paymentHistoryCacheMock = new Mock<IRepositoryCache<PaymentHistoryEntity[]>>(MockBehavior.Strict);
            apprenticeshipKeyServiceMock = new Mock<IApprenticeshipKeyService>(MockBehavior.Strict);
            paymentHistoryRepositoryMock = new Mock<IPaymentHistoryRepository>(MockBehavior.Strict);

            act2PaymentDueEventHandler = new ApprenticeshipContractType2PaymentDueEventHandler(
                paymentDueProcessorMock.Object,
                paymentHistoryCacheMock.Object,
                Mapper.Instance,
                apprenticeshipKeyServiceMock.Object,
                paymentHistoryRepositoryMock.Object,
                "key"
            );
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

            apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
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

            apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
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
            var paymentHistory = new[]
            {
                new PaymentHistoryEntity
                {
                    PriceEpisodeIdentifier = "1",
                    LearnAimReference = "2",
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
                    LearnAimReference = "2",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R01"
                }
            };

            paymentHistoryRepositoryMock.Setup(r => r.GetPaymentHistory("key", It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();

            apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("1", "2", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "2", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("3", "2", 3, It.IsAny<CalendarPeriod>())).Returns("2").Verifiable();

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
                AgreedPrice = 10,
                FundingLineType = "11"
            };
        }

        private static Learner CreateLearner()
        {
            return new Learner
            {
                Ukprn = 1,
                ReferenceNumber = "3",
                Uln = 4
            };
        }
    }
}
