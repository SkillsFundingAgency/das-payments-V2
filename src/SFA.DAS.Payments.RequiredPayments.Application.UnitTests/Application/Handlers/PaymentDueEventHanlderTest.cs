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
    public class PaymentDueEventHanlderTest
    {
        private IPaymentDueEventHanlder _paymentDueEventHandler;
        private Mock<IPaymentDueProcessor> _paymentDueProcessorMock;
        private Mock<IRepositoryCache<PaymentEntity[]>> _paymentHistoryCacheMock;
        private Mock<IApprenticeshipKeyService> _apprenticeshipKeyServiceMock;

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
            _paymentDueProcessorMock = new Mock<IPaymentDueProcessor>(MockBehavior.Strict);
            _paymentHistoryCacheMock = new Mock<IRepositoryCache<PaymentEntity[]>>(MockBehavior.Strict);
            _apprenticeshipKeyServiceMock = new Mock<IApprenticeshipKeyService>(MockBehavior.Strict);
        }

        [Test]
        public async Task TestHandleNullEvent()
        {
            // arrange            
            _paymentDueEventHandler = new PaymentDueEventHanlder(_paymentDueProcessorMock.Object, _paymentHistoryCacheMock.Object, Mapper.Instance, _apprenticeshipKeyServiceMock.Object);

            // act
            // assert
            try
            {
                await _paymentDueEventHandler.HandlePaymentDue(null);
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
            var paymentDue = new OnProgrammePaymentDueEvent
            {
                Ukprn = 1,
                PriceEpisodeIdentifier = "2",
                Amount = 100,
                DeliveryPeriod = new NamedCalendarPeriod {Year = 2018, Month = 9, Name = "1819R02"},
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
            var requiredPayment = new RequiredPaymentEvent
            {
                Amount = 1,
                JobId = "2"
            };

            _apprenticeshipKeyServiceMock.Setup(s => s.GeneratePaymentKey("2", "9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            _paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentEntity[]>(true, paymentHistoryEntities)).Verifiable();
            _paymentDueProcessorMock.Setup(p => p.ProcessPaymentDue(paymentDue, It.IsAny<Payment[]>())).Returns(requiredPayment).Verifiable();
            _paymentDueEventHandler = new PaymentDueEventHanlder(_paymentDueProcessorMock.Object, _paymentHistoryCacheMock.Object, Mapper.Instance, _apprenticeshipKeyServiceMock.Object);

            // act
            var actualRequiredPayment = await _paymentDueEventHandler.HandlePaymentDue(paymentDue);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
        }
    }
}
