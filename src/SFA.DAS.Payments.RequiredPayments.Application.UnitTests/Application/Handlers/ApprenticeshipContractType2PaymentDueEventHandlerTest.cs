using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.Moq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Application.Handlers;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Handlers
{
    [TestFixture]
    public class ApprenticeshipContractType2PaymentDueEventHandlerTest
    {
        private AutoMock mocker;
        private ApprenticeshipContractType2PaymentDueEventHandler act2PaymentDueEventHandler;
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

            act2PaymentDueEventHandler = mocker.Create<ApprenticeshipContractType2PaymentDueEventHandler>(
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
                await act2PaymentDueEventHandler.HandlePaymentDue(null, paymentHistoryCacheMock.Object, CancellationToken.None);
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
                CollectionPeriod = new CollectionPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                DeliveryPeriod = new DeliveryPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Learning
            };

            var paymentHistoryEntities = new[] { new PaymentHistoryEntity
            {
                CollectionPeriod = CollectionPeriod.CreateFromAcademicYearAndPeriod("1819", 2),
                DeliveryPeriod = DeliveryPeriod.CreateFromAcademicYearAndPeriod("1819", 2),
            } };

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(1).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue, paymentHistoryCacheMock.Object, CancellationToken.None);

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
                CollectionPeriod = new CollectionPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                DeliveryPeriod = new DeliveryPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Learning
            };

            var paymentHistoryEntities = new PaymentHistoryEntity[0];

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey("9", 1, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(100, It.IsAny<Payment[]>())).Returns(0).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNull(actualRequiredPayment);
        }

        [Test]
        [TestCase(0, "1")]
        [TestCase(0, null)]
        [TestCase(50, "1")]
        public async Task TestPriceEpisodeIdentifierPickedFromHistoryForRefunds(decimal amount, string priceEpisodeIdentifier)
        {
            // arrange
            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                Ukprn = 1,
                PriceEpisodeIdentifier = priceEpisodeIdentifier,
                AmountDue = amount,
                CollectionPeriod = new CollectionPeriodBuilder().WithYear(2018).WithMonth(10).Build(),
                DeliveryPeriod = new DeliveryPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Balancing
            };

            var paymentHistoryEntities = new []
            {
                new PaymentHistoryEntity
                {
                    Amount = 100,
                    PriceEpisodeIdentifier = "2",
                    CollectionPeriod = new CollectionPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                    DeliveryPeriod = new DeliveryPeriodBuilder().WithYear(2018).WithMonth(9).Build(),
                    TransactionType = (int)IncentiveEarningType.Balancing16To18FrameworkUplift,
                    Ukprn = 1,
                    LearnAimReference = paymentDue.LearningAim.Reference,
                    LearnerReferenceNumber = paymentDue.Learner.ReferenceNumber,
                    ExternalId = Guid.NewGuid()
                }
            };

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey(paymentDue.LearningAim.Reference, (int)OnProgrammeEarningType.Balancing, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities)).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(amount, It.IsAny<Payment[]>())).Returns(-100).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual("2", actualRequiredPayment.PriceEpisodeIdentifier);

        }

        [Test]
        [TestCase(null, null, null, 0)]
        [TestCase(900, null, null, 1)]
        [TestCase(900, 100, null, .9)]
        [TestCase(900, 50, 50, .9)]
        public async Task TestSfaContributionIsCalculatedForZeroEarningRefunds(decimal? sfaContribution, decimal? employerContribution1, decimal? employerContribution2, decimal expectedPercent)
        {
            // arrange
            var paymentDue = new ApprenticeshipContractType2PaymentDueEvent
            {
                Ukprn = 1,
                PriceEpisodeIdentifier = "priceEpisodeIdentifier",
                AmountDue = 0,
                SfaContributionPercentage = 0,
                CollectionPeriod = new CalendarPeriod(2018, 10),
                DeliveryPeriod = new CalendarPeriod(2018, 9),
                Learner = CreateLearner(),
                LearningAim = CreateLearningAim(),
                Type = OnProgrammeEarningType.Balancing
            };

            var paymentHistoryEntities = new List<PaymentHistoryEntity>();

            if (sfaContribution.HasValue)
                paymentHistoryEntities.Add(CreatePaymentEntity(sfaContribution.Value, paymentDue, FundingSourceType.CoInvestedSfa));

            if (employerContribution1.HasValue)
                paymentHistoryEntities.Add(CreatePaymentEntity(employerContribution1.Value, paymentDue, FundingSourceType.CoInvestedEmployer));

            if (employerContribution2.HasValue)
                paymentHistoryEntities.Add(CreatePaymentEntity(employerContribution2.Value, paymentDue, FundingSourceType.CoInvestedEmployer));

            mocker.Mock<IPaymentKeyService>().Setup(s => s.GeneratePaymentKey(paymentDue.LearningAim.Reference, (int)OnProgrammeEarningType.Balancing, paymentDue.DeliveryPeriod)).Returns("payment key").Verifiable();
            paymentHistoryCacheMock.Setup(c => c.TryGet("payment key", It.IsAny<CancellationToken>())).ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, paymentHistoryEntities.ToArray())).Verifiable();
            paymentDueProcessorMock.Setup(p => p.CalculateRequiredPaymentAmount(0, It.IsAny<Payment[]>())).Returns(-100).Verifiable();

            // act
            var actualRequiredPayment = await act2PaymentDueEventHandler.HandlePaymentDue(paymentDue, paymentHistoryCacheMock.Object, CancellationToken.None);

            // assert
            Assert.IsNotNull(actualRequiredPayment);
            Assert.AreEqual(expectedPercent, ((ApprenticeshipContractType2RequiredPaymentEvent)actualRequiredPayment).SfaContributionPercentage);
        }

        private static PaymentHistoryEntity CreatePaymentEntity(decimal amount, ApprenticeshipContractType2PaymentDueEvent paymentDue, FundingSourceType fundingSourceType)
        {
            return new PaymentHistoryEntity
            {
                Amount = amount,
                FundingSource = fundingSourceType,
                TransactionType = (int)OnProgrammeEarningType.Learning,
                PriceEpisodeIdentifier = "2",
                CollectionPeriod = new CalendarPeriod(2018, 9).Name,
                DeliveryPeriod = new CalendarPeriod(2018, 9).Name,
                Ukprn = 1,
                LearnAimReference = paymentDue.LearningAim.Reference,
                LearnerReferenceNumber = paymentDue.Learner.ReferenceNumber,
                ExternalId = Guid.NewGuid()

            };
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