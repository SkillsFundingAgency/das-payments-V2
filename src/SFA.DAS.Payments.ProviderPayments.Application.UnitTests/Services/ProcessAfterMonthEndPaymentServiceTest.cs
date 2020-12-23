using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class ProcessAfterMonthEndPaymentServiceTest
    {
        private IPaymentLogger paymentLogger;
        private Mock<IMapper> mapper;
        private Mock<IProviderPeriodEndService> monthEndService;
        private EmployerCoInvestedFundingSourcePaymentEvent fundingSourcePaymentEvent;
        private ProviderPaymentEvent providerPaymentEvent;

        [SetUp]
        public void SetUp()
        {
            fundingSourcePaymentEvent = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                EventId = Guid.NewGuid(),
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
                RequiredPaymentEventId = Guid.NewGuid()
            };
            providerPaymentEvent = new EmployerCoInvestedProviderPaymentEvent
            {
                EventId = Guid.NewGuid(),
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
            };

            paymentLogger = Mock.Of<IPaymentLogger>();
            mapper = new Mock<IMapper>(MockBehavior.Strict);
            mapper.Setup(o => o.Map<ProviderPaymentEvent>(It.IsAny<FundingSourcePaymentEvent>()))
                .Returns(providerPaymentEvent);

            monthEndService = new Mock<IProviderPeriodEndService>();
            monthEndService
                .Setup(o => o.IsMonthEndStarted(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(true);

            monthEndService
                .Setup(o => o.GetMonthEndJobId(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(providerPaymentEvent.JobId);

        }

        [Test]
        public async Task GetPaymentEventShouldReturnPaymentIfIsMonthEnd()
        {
            var handleAfterMonthEndServiceTest = new ProcessAfterMonthEndPaymentService(paymentLogger, mapper.Object, monthEndService.Object);
            var payment = await handleAfterMonthEndServiceTest.GetPaymentEvent(fundingSourcePaymentEvent);
            payment.Should().NotBeNull();
            payment.Should().BeOfType<EmployerCoInvestedProviderPaymentEvent>();
            payment.JobId.Should().Be(providerPaymentEvent.JobId);
            payment.Ukprn.Should().Be(providerPaymentEvent.Ukprn);
        }

        [Test]
        public async Task GetPaymentEventShouldNotReturnPaymentIfIsNotMonthEnd()
        {
            monthEndService
                .Setup(o => o.IsMonthEndStarted(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>()))
                .ReturnsAsync(false);

            var handleAfterMonthEndServiceTest = new ProcessAfterMonthEndPaymentService(paymentLogger, mapper.Object, monthEndService.Object);
            var payment = await handleAfterMonthEndServiceTest.GetPaymentEvent(fundingSourcePaymentEvent);
            payment.Should().BeNull();
        }

    }
}
