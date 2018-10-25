using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using SFA.DAS.Payments.ProviderPayments.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class ProviderPaymentsHandlerServiceTest
    {
        private ProviderPaymentsHandlerService providerPaymentsHandlerService;

        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;
        private Mock<IDataCache<IlrSubmittedEvent>> ilrSubmittedEventCache;
        private Mock<IValidatePaymentMessage> validatePaymentMessage;
        private Mock<IPaymentLogger> paymentLogger;

        private long ukprn = 10000;
        private long jobId = 10000;

        private ProviderPeriodicPayment fundingSourceEvent;
        private List<PaymentModel> payments;

        [SetUp]
        public void SetUp()
        {
            fundingSourceEvent = new ProviderPeriodicPayment
            {
                ContractType = 2,
                FundingSourceType = FundingSourceType.CoInvestedSfa,
                OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                CollectionPeriod = new CalendarPeriod(2018, 10),
                DeliveryPeriod = new CalendarPeriod(2018, 11),
                LearningAim = new LearningAim
                {
                    FrameworkCode = 1,
                    Reference = "100",
                    PathwayCode = 1,
                    StandardCode = 1,
                    ProgrammeType = 1,
                    AgreedPrice = 1000m,
                    FundingLineType = "T"
                },
                Learner = new Learner
                {
                    Ukprn = ukprn,
                    ReferenceNumber = "A1000",
                    Uln = 10000000
                },
                Ukprn = ukprn,
                SfaContributionPercentage = 0.9m,
                AmountDue = 1000m,
                JobId = jobId
            };


            payments = new List<PaymentModel>
            {
                new PaymentModel()
                {
                    Ukprn = 1000,
                    FundingSource = (int)FundingSourceType.CoInvestedEmployer,
                    SfaContributionPercentage = 0.9m,
                    JobId = 1,
                    DeliveryPeriodYear = 2018,
                    IlrSubmissionDateTime = DateTime.MinValue,
                    DeliveryPeriodMonth = 2,
                    ContractType = 2,
                    CollectionPeriodYear = 2018,
                    PriceEpisodeIdentifier = "P-1",
                    LearnerReferenceNumber = "100500",
                    CollectionPeriodMonth = 10,
                    ExternalId = Guid.NewGuid(),
                    LearningAimFundingLineType = "16-18",
                    LearningAimPathwayCode = 1,
                    LearningAimReference = "1",
                    LearningAimFrameworkCode = 1,
                    LearningAimAgreedPrice = 200.05m,
                    TransactionType = 1,
                    LearnerUln = 1000,
                    LearningAimProgrammeType = 1,
                    CollectionPeriodName = "1819-R03",
                    LearningAimStandardCode = 1,
                    Amount = 2000.00m
                }
            };

            providerPaymentsRepository = new Mock<IProviderPaymentsRepository>();
            providerPaymentsRepository
                .Setup(t => t.SavePayment(It.IsAny<PaymentModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            providerPaymentsRepository
                          .Setup(o => o.GetMonthEndPayments(It.IsAny<short>(), It.IsAny<byte>(), It.IsAny<long>(), It.IsAny<CancellationToken>()))
                          .ReturnsAsync(payments)
                          .Verifiable();


            var ilrSubmittedEvent = new IlrSubmittedEvent
            {
                Ukprn = ukprn,
                JobId = jobId,
                IlrSubmissionDateTime = DateTime.MaxValue
            };

            ilrSubmittedEventCache = new Mock<IDataCache<IlrSubmittedEvent>>();
            ilrSubmittedEventCache
                .Setup(o => o.TryGet(ukprn.ToString(), default(CancellationToken)))
                .Returns(Task.FromResult(new ConditionalValue<IlrSubmittedEvent>(true, ilrSubmittedEvent)));

            validatePaymentMessage = new Mock<IValidatePaymentMessage>();
            validatePaymentMessage
                .Setup(o => o.IsLatestIlrPayment(It.IsAny<PaymentMessageValidationRequest>()))
                .Returns(true);

            paymentLogger = new Mock<IPaymentLogger>();
            paymentLogger
                .Setup(o => o.LogDebug(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Verifiable();

            paymentLogger
                .Setup(o => o.LogWarning(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Verifiable();
            
            providerPaymentsHandlerService = new ProviderPaymentsHandlerService(providerPaymentsRepository.Object, ilrSubmittedEventCache.Object, validatePaymentMessage.Object, paymentLogger.Object);
        }

        [Test]
        public async Task ProcessEventShouldCallRequiredServices()
        {
            await providerPaymentsHandlerService.ProcessPayment(fundingSourceEvent, default(CancellationToken));

            providerPaymentsRepository
                .Verify(o => o.SavePayment(It.IsAny<PaymentModel>(), default(CancellationToken)), Times.Once);

            ilrSubmittedEventCache
                .Verify(o => o.TryGet(ukprn.ToString(), default(CancellationToken)), Times.Once);

            validatePaymentMessage
                .Verify(o => o.IsLatestIlrPayment(It.IsAny<PaymentMessageValidationRequest>()), Times.Once);
        }

        [Test]
        public async Task ProcessEventShouldNotCallRepositoryIfPaymentEventIsInvalid()
        {

            validatePaymentMessage
                .Setup(o => o.IsLatestIlrPayment(It.IsAny<PaymentMessageValidationRequest>()))
                .Returns(false);


            await providerPaymentsHandlerService.ProcessPayment(fundingSourceEvent, default(CancellationToken));

            providerPaymentsRepository
                .Verify(o => o.SavePayment(It.IsAny<PaymentModel>(), default(CancellationToken)), Times.Never);

        }

        [Test]
        public async Task ProcessEventShouldWriteToLogIfPaymentEventIsInvalid()
        {

            validatePaymentMessage
                .Setup(o => o.IsLatestIlrPayment(It.IsAny<PaymentMessageValidationRequest>()))
                .Returns(false);

            await providerPaymentsHandlerService.ProcessPayment(fundingSourceEvent, default(CancellationToken));

            paymentLogger
                .Verify(o => o.LogWarning(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()));

        }


        [Test]
        public async Task GetMonthEndPaymentsShouldReturnPaymentsFromRepository()
        {
            short year = 2018;
            byte month = 9;
            long ukprn = 1000;
            var cancellationToken = new CancellationToken();

            var results = await providerPaymentsHandlerService.GetMonthEndPayments(year, month, ukprn, cancellationToken);

            Assert.IsNotNull(results);
            providerPaymentsRepository.Verify(o => o.GetMonthEndPayments(It.IsAny<short>(), 
                                                    It.IsAny<byte>(), 
                                                    It.IsAny<long>(),
                                                    It.IsAny<CancellationToken>()), Times.Once);
        }



    }
}
