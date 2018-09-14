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
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Handlers
{
    [TestFixture]
    public class Act2PaymentDueEventHanlderTest
    {
        private IAct2PaymentDueEventHanlder _act2PaymentDueEventHandler;
        private Mock<IAct2PaymentDueProcessor> _paymentDueProcessorMock;
        private Mock<IRepositoryCache<PaymentEntity[]>> _paymentHistoryCacheMock;
        private Mock<IApprenticeshipKeyService> _apprenticeshipKeyServiceMock;
        private Mock<IPaymentHistoryRepository> _paymentHistoryRepositoryMock;

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
            _paymentDueProcessorMock = new Mock<IAct2PaymentDueProcessor>(MockBehavior.Strict);
            _paymentHistoryCacheMock = new Mock<IRepositoryCache<PaymentEntity[]>>(MockBehavior.Strict);
            _apprenticeshipKeyServiceMock = new Mock<IApprenticeshipKeyService>(MockBehavior.Strict);
            _paymentHistoryRepositoryMock = new Mock<IPaymentHistoryRepository>(MockBehavior.Strict);

            _act2PaymentDueEventHandler = new Act2PaymentDueEventHanlder(
                _paymentDueProcessorMock.Object,
                _paymentHistoryCacheMock.Object,
                Mapper.Instance,
                _apprenticeshipKeyServiceMock.Object,
                _paymentHistoryRepositoryMock.Object,
                "key"
            );
        }

        [TearDown]
        public void TearDown()
        {
            _paymentDueProcessorMock.Verify();
            _paymentHistoryCacheMock.Verify();
            _apprenticeshipKeyServiceMock.Verify();
            _paymentHistoryRepositoryMock.Verify();
        }

        [Test]
        public async Task TestHandleNullEvent()
        {
            // arrange            
            // act
            // assert
            try
            {
                await _act2PaymentDueEventHandler.HandlePaymentDue(null);
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
                DeliveryPeriod = new CalendarPeriod {Year = 2018, Month = 9, Name = "1819R02"},
                Learner = new Learner
                {
                    Ukprn = 1,
                    ReferenceNumber = "3",
                    Uln = 4
                },
                LearningAim = new LearningAim
                {
                    ProgrammeType = 5,
                    PathwayCode = 6,
                    StandardCode = 7,
                    FrameworkCode = 8,
                    Reference = "9",
                    AgreedPrice = 10,
                    FundingLineType = "11"
                },
                OnProgrammeEarningType = OnProgrammeEarningType.Learning
            };

            var paymentHistoryEntities = new[] { new PaymentEntity() }; 
            var requiredPayment = new ApprenticeshipContractType2RequiredPaymentEvent
            {
                AmountDue = 1,
                JobId = "2"
            };

            _apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            _paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentEntity[]>(true, paymentHistoryEntities)).Verifiable();
            _paymentDueProcessorMock.Setup(p => p.ProcessPaymentDue(paymentDue, It.IsAny<Payment[]>())).Returns(requiredPayment).Verifiable();

            // act
            var actualRequiredPayment = await _act2PaymentDueEventHandler.HandlePaymentDue(paymentDue);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
        }

        [Test]
        public async Task TestPopulateHistory()
        {
            // arrange
            var paymentHistory = new[]
            {
                new PaymentEntity
                {
                    PriceEpisodeIdentifier = "1",
                    LearnAimReference = "2",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R01"
                },
                new PaymentEntity
                {
                    PriceEpisodeIdentifier = "2",
                    LearnAimReference = "2",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R02"
                },
                new PaymentEntity
                {
                    PriceEpisodeIdentifier = "3",
                    LearnAimReference = "2",
                    TransactionType = 3,
                    DeliveryPeriod = "1819R01"
                }
            };

            _paymentHistoryRepositoryMock.Setup(r => r.GetPaymentHistory("key", It.IsAny<CancellationToken>())).ReturnsAsync(paymentHistory).Verifiable();

            _apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("1", "2", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            _apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "2", 3, It.IsAny<CalendarPeriod>())).Returns("1").Verifiable();
            _apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("3", "2", 3, It.IsAny<CalendarPeriod>())).Returns("2").Verifiable();

            _paymentHistoryCacheMock.Setup(c => c.Add("1", It.Is<PaymentEntity[]>(p => p.Length == 2 && p[0].PriceEpisodeIdentifier == "1" && p[1].PriceEpisodeIdentifier == "2"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Verifiable();
            _paymentHistoryCacheMock.Setup(c => c.Add("2", It.Is<PaymentEntity[]>(p => p.Length == 1 && p[0].PriceEpisodeIdentifier == "3"), It.IsAny<CancellationToken>())).Returns(Task.FromResult(0)).Verifiable();


            // act
            await _act2PaymentDueEventHandler.PopulatePaymentHistoryCache(CancellationToken.None).ConfigureAwait(false);

            // assert
        }
    }
}
