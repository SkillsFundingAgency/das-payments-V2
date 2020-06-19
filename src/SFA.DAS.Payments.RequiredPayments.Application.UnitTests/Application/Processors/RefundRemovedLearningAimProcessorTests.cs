using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using ESFA.DC.Logging.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Domain.Services;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class RefundRemovedLearningAimProcessorTests
    {
        private AutoMock mocker;
        private List<PaymentHistoryEntity> history;
        private IdentifiedRemovedLearningAim identifiedLearner;
        private Mock<IDuplicateEarningEventService> duplicateEarningsServiceMock;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            history = new List<PaymentHistoryEntity>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            var mapper = new Mapper(config);
            mocker.Provide<IMapper>(mapper);
            var logger = new Mock<IPaymentLogger>();

            duplicateEarningsServiceMock = mocker.Mock<IDuplicateEarningEventService>();
            mocker.Provide<IRefundRemovedLearningAimService>(new RefundRemovedLearningAimService());
            mocker.Provide<IPeriodisedRequiredPaymentEventFactory>(new PeriodisedRequiredPaymentEventFactory(logger.Object));

            identifiedLearner = new IdentifiedRemovedLearningAim
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "learner-ref-123",
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learning-ref-456",
                    StandardCode = 6
                },
                Ukprn = 7
            };
        }

        private PaymentHistoryEntity CreatePaymentHistoryEntity(FundingSourceType fundingSource, byte deliveryPeriod, TransactionType transactionType = TransactionType.Learning)
        {
            return new PaymentHistoryEntity
            {
                Amount = 10,
                SfaContributionPercentage = .9M,
                TransactionType = (int)transactionType,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                LearnAimReference = "ZPROG001",
                LearnerReferenceNumber = "learning-ref-456",
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = deliveryPeriod,
                Ukprn = 7,
                LearnerUln = 5,
                ActualEndDate = null,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                ExternalId = Guid.NewGuid(),
                FundingSource = fundingSource,
                InstalmentAmount = 1000,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddMonths(-12),
            };

        }


        [Test]
        public async Task RefundsCorrectTypesBasedOnHistory()
        {
            var historicalCompletionPayment =
                CreatePaymentHistoryEntity(FundingSourceType.CoInvestedSfa, 1, TransactionType.Completion);
            historicalCompletionPayment.Amount = 1000;

            var historicalBalancingPayment =
                CreatePaymentHistoryEntity(FundingSourceType.CoInvestedSfa, 1, TransactionType.Balancing);
            historicalBalancingPayment.Amount = 166.66m;

            var historicalCompletionUplift = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 1,
                TransactionType.Completion16To18FrameworkUplift);
            historicalCompletionUplift.Amount = 200m;

            var historicalBalancingUpliftPayment = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 1,
                TransactionType.Balancing16To18FrameworkUplift);
            historicalBalancingUpliftPayment.Amount = 33.33m;

            history.Add(historicalCompletionPayment);
            history.Add(historicalBalancingPayment);
            history.Add(historicalCompletionUplift);
            history.Add(historicalBalancingUpliftPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            mocker.Provide<IPeriodisedRequiredPaymentEventFactory, PeriodisedRequiredPaymentEventFactory>();
            mocker.Provide<IRefundService, RefundService>();
            mocker.Provide<IPaymentDueProcessor, PaymentDueProcessor>();
            mocker.Provide<IRefundRemovedLearningAimService, RefundRemovedLearningAimService>();


            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner,
                mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(4);
            refunds[0].Should().BeOfType<CalculatedRequiredCoInvestedAmount>();
            refunds[0].AmountDue.Should().Be(-1 * historicalCompletionPayment.Amount);
            refunds[1].Should().BeOfType<CalculatedRequiredCoInvestedAmount>();
            refunds[1].AmountDue.Should().Be(-1 * historicalBalancingPayment.Amount);
            refunds[2].Should().BeOfType<CalculatedRequiredIncentiveAmount>();
            refunds[2].AmountDue.Should().Be(-1 * historicalCompletionUplift.Amount);
            refunds[3].Should().BeOfType<CalculatedRequiredIncentiveAmount>();
            refunds[3].AmountDue.Should().Be(-1 * historicalBalancingUpliftPayment.Amount);
        }

        [Test]
        public async Task ReturnsNullAndLogsWhenInvalidTransactionTypeForFundingSource()
        {
            var mockLogger = mocker.Mock<IPaymentLogger>();
            mockLogger.Setup(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>(),
                 It.IsAny<long>(),
                 It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Verifiable();

            history.Add(CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 1, TransactionType.Completion));

            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            mocker.Provide<IPeriodisedRequiredPaymentEventFactory, PeriodisedRequiredPaymentEventFactory>();
            mocker.Provide<IRefundService, RefundService>();
            mocker.Provide<IPaymentDueProcessor, PaymentDueProcessor>();
            mocker.Provide<IRefundRemovedLearningAimService, RefundRemovedLearningAimService>();

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner,
                mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);

            refunds.Should().HaveCount(0);

            mockLogger.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>(),
                It.IsAny<long>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once); ;

        }

        [Test]
        public async Task Refunds_Required_Levy_Payments()
        {
            var historicPayment = CreatePaymentHistoryEntity(FundingSourceType.Levy, 1);
            history.Add(historicPayment);

            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None)
                .ConfigureAwait(false);

            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            var levyRefund = refund as CalculatedRequiredLevyAmount;
            levyRefund.Should().NotBeNull();
            levyRefund.AmountDue.Should().Be(-10);
            levyRefund.ShouldBeMappedTo(historicPayment);
            levyRefund.ShouldBeMappedTo(identifiedLearner);
            levyRefund.DeliveryPeriod.Should().Be(1);
        }

        [Test]
        public async Task Refunds_Required_CoInvested_Payments()
        {
            var historicPayment = CreatePaymentHistoryEntity(FundingSourceType.CoInvestedSfa, 2);
            history.Add(historicPayment);

            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));


            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            var conInvestRefund = refund as CalculatedRequiredCoInvestedAmount;
            conInvestRefund.Should().NotBeNull();
            conInvestRefund.AmountDue.Should().Be(-10);
            conInvestRefund.ShouldBeMappedTo(historicPayment);
            conInvestRefund.ShouldBeMappedTo(identifiedLearner);
            conInvestRefund.DeliveryPeriod.Should().Be(2);
        }

        [Test]
        public async Task Refunds_Required_Incentive_Payments()
        {
            var historicPayment = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 3, TransactionType.OnProgrammeMathsAndEnglish);
            history.Add(historicPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            var incentiveRefund = refund as CalculatedRequiredIncentiveAmount;
            incentiveRefund.Should().NotBeNull();
            incentiveRefund.AmountDue.Should().Be(-10);
            incentiveRefund.ShouldBeMappedTo(historicPayment);
            incentiveRefund.ShouldBeMappedTo(identifiedLearner);
            incentiveRefund.DeliveryPeriod.Should().Be(3);
        }

        [Test]
        public async Task DoesNotProcessDuplicatesTwice()
        {
            var historicPayment = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 3, TransactionType.OnProgrammeMathsAndEnglish);
            history.Add(historicPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            duplicateEarningsServiceMock.Setup(x => x.IsDuplicate(It.IsAny<IPaymentsEvent>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var sut = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await sut.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Should().HaveCount(0);
        }

        [Test]
        public async Task Refunds_All_Required_Payments()
        {
            var historicLevyPayment = CreatePaymentHistoryEntity(FundingSourceType.Levy, 4);
            historicLevyPayment.Amount = 100;
            var historicCoInvestedPayment = CreatePaymentHistoryEntity(FundingSourceType.CoInvestedSfa, 4);
            historicCoInvestedPayment.Amount = 50;
            var historicIncentivePayment = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 4, TransactionType.OnProgrammeMathsAndEnglish);
            historicIncentivePayment.Amount = 25;
            history.Add(historicLevyPayment);
            history.Add(historicCoInvestedPayment);
            history.Add(historicIncentivePayment);

            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(3);
            refunds.Count(refund => refund.AmountDue == historicLevyPayment.Amount * -1 && refund is CalculatedRequiredLevyAmount).Should().Be(1);
            refunds.Count(refund => refund.AmountDue == historicCoInvestedPayment.Amount * -1 && refund is CalculatedRequiredCoInvestedAmount).Should().Be(1);
            refunds.Count(refund => refund.AmountDue == historicIncentivePayment.Amount * -1 && refund is CalculatedRequiredIncentiveAmount).Should().Be(1);
            refunds.Count(refund => refund.DeliveryPeriod == 4).Should().Be(3);
        }

        [Test]
        public async Task Generate_Valid_SfaContribution_For_CoInvested_Refund_Payment()
        {
            var historicIncentivePayment = CreatePaymentHistoryEntity(FundingSourceType.FullyFundedSfa, 2);
            historicIncentivePayment.Amount = 500m;
            historicIncentivePayment.SfaContributionPercentage = 0m;
            historicIncentivePayment.ContractType = ContractType.Act2;
            historicIncentivePayment.TransactionType = (int)TransactionType.Second16To18EmployerIncentive;
            historicIncentivePayment.CollectionPeriod = new CollectionPeriod
            {
                AcademicYear = 1920,
                Period = 3
            };
            history.Add(historicIncentivePayment);

            var historicCoInvestedPayment = CreatePaymentHistoryEntity(FundingSourceType.CoInvestedSfa, 2);
            historicCoInvestedPayment.Amount = 160;
            historicCoInvestedPayment.SfaContributionPercentage = 1m;
            historicCoInvestedPayment.ContractType = ContractType.Act2;
            historicCoInvestedPayment.TransactionType = (int)TransactionType.Learning;
            historicCoInvestedPayment.CollectionPeriod = new CollectionPeriod
            {
                AcademicYear = 1920,
                Period = 3
            };
            history.Add(historicCoInvestedPayment);


            identifiedLearner.CollectionPeriod = new CollectionPeriod
            {
                AcademicYear = 1920,
                Period = 5
            };

            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));


            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner,
                mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object,
                CancellationToken.None)
                .ConfigureAwait(false);

            var refund = refunds.FirstOrDefault(o => o is CalculatedRequiredCoInvestedAmount);
            refund.Should().NotBeNull();

            var coInvestedAmountPayment = refund as CalculatedRequiredCoInvestedAmount;
            coInvestedAmountPayment.SfaContributionPercentage.Should().Be(1m);
        }
    }
}