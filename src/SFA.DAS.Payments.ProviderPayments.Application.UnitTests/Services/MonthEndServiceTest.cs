using Autofac.Extras.Moq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Repositories;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Domain;
using SFA.DAS.Payments.ProviderPayments.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Payments.ProviderPayments.Model;
using SFA.DAS.Payments.Tests.Core.Builders;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Services
{
    [TestFixture]
    public class MonthEndServiceTests
    {
        private AutoMock mocker;
        private MonthEndService monthEndService;

        private Mock<IProviderPaymentsRepository> providerPaymentsRepository;
        private Mock<IDataCache<ReceivedProviderEarningsEvent>> ilrSubmittedEventCache;
        private Mock<IValidateIlrSubmission> validateIlrSubmission;
        
        private long ukprn = 10000;
        private long jobId = 10000;
        private List<PaymentModel> payments;
        private ReceivedProviderEarningsEvent receivedProviderEarningsEvent;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            payments = new List<PaymentModel>
            {
                new PaymentModel()
                {
                    Ukprn = ukprn,
                    FundingSource = FundingSourceType.CoInvestedEmployer,
                    SfaContributionPercentage = 0.9m,
                    JobId = 1,
                    DeliveryPeriod = 7,
                    CollectionPeriod = new CollectionPeriod
                    {
                        AcademicYear = 1819,
                        Period = 8
                    },
                    IlrSubmissionDateTime = DateTime.UtcNow,
                    ContractType = ContractType.Act1,
                    PriceEpisodeIdentifier = "P-1",
                    LearnerReferenceNumber = "100500",
                    EventId = Guid.NewGuid(),
                    LearningAimFundingLineType = "16-18",
                    LearningAimPathwayCode = 1,
                    LearningAimReference = "1",
                    LearningAimFrameworkCode = 1,
                    TransactionType = TransactionType.Learning,
                    LearnerUln = 1000,
                    LearningAimProgrammeType = 1,
                    LearningAimStandardCode = 1,
                    Amount = 2000.00m
                }
            };

            providerPaymentsRepository = mocker.Mock<IProviderPaymentsRepository>();
            providerPaymentsRepository
                .Setup(t => t.SavePayment(It.IsAny<PaymentModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            providerPaymentsRepository
                          .Setup(o => o.GetMonthEndPayments(It.IsAny<CollectionPeriod>(), 
                              It.IsAny<long>(), 
                              It.IsAny<CancellationToken>()))
                          .ReturnsAsync(payments)
                          .Verifiable();

            providerPaymentsRepository
                .Setup(o => o.DeleteOldMonthEndPayment(It.IsAny<CollectionPeriod>(),
                                                        It.IsAny<long>(),
                                                        It.IsAny<DateTime>(),
                                                        It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            receivedProviderEarningsEvent = new ReceivedProviderEarningsEvent
            {
                Ukprn = ukprn,
                JobId = jobId,
                IlrSubmissionDateTime = DateTime.MaxValue,
                CollectionPeriod = new CollectionPeriodBuilder().WithDate(new DateTime(2018, 2, 1)).Build(),
            };

            ilrSubmittedEventCache = mocker.Mock<IDataCache<ReceivedProviderEarningsEvent>>();
            ilrSubmittedEventCache
                .Setup(o => o.TryGet(ukprn.ToString(), default(CancellationToken)))
                .Returns(Task.FromResult(new ConditionalValue<ReceivedProviderEarningsEvent>(true, receivedProviderEarningsEvent)));

            ilrSubmittedEventCache
                .Setup(o => o.Clear(ukprn.ToString(), default(CancellationToken)))
                .Returns(Task.CompletedTask);

            ilrSubmittedEventCache
                .Setup(o => o.Add(ukprn.ToString(), 
                    It.IsAny<ReceivedProviderEarningsEvent>(), 
                    default(CancellationToken)))
                .Returns(Task.CompletedTask);

            validateIlrSubmission = mocker.Mock<IValidateIlrSubmission>();
            validateIlrSubmission
                .Setup(o => o.IsLatestIlrPayment(It.IsAny<IlrSubmissionValidationRequest>()))
                .Returns(true);

            mocker.Mock<IMonthEndCache>()
                .Setup(cache => cache.AddOrReplace(It.IsAny<long>(),
                    It.IsAny<short>(), 
                    It.IsAny<byte>(),
                    It.IsAny<long>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            monthEndService = mocker.Create<MonthEndService>();
        }

        [Test]
        public async Task GetMonthEndPaymentsShouldReturnPaymentsFromRepository()
        {
            var cancellationToken = new CancellationToken();

            var results = await monthEndService.GetMonthEndPayments(new CollectionPeriod{Period = 2, AcademicYear = 1819}, ukprn, cancellationToken);

            Assert.IsNotNull(results);
            providerPaymentsRepository.Verify(o => o.GetMonthEndPayments(It.Is<CollectionPeriod>(cp => cp.AcademicYear == 1819 && cp.Period == 2),
                                                    It.Is<long>(x => x == ukprn),
                                                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task RecordMonthEndStoresMonthEndInfoInCache()
        {
            await monthEndService.StartMonthEnd(ukprn, 1819, 2,jobId);
            mocker.Mock<IMonthEndCache>()
                .Verify(cache => cache.AddOrReplace(It.Is<long>(expectedUkprn => expectedUkprn == ukprn),
                    It.Is<short>(academicYear => academicYear == 1819),
                    It.Is<byte>(collectionPeriod => collectionPeriod == 2),
                    It.Is<long>(monthEndJobId => monthEndJobId == jobId),
                    It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task MonthEndStartedReturnsTrueIfMonthEndRecorded()
        {
            mocker.Mock<IMonthEndCache>()
                .Setup(cache => cache.Exists(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));
            var started = await monthEndService.MonthEndStarted(ukprn, 1819, 2);
            started.Should().BeTrue();
        }

        [Test]
        public async Task MonthEndStartedReturnsFalseIfMonthEndNotRecorded()
        {
            mocker.Mock<IMonthEndCache>()
                .Setup(cache => cache.Exists(It.IsAny<long>(), It.IsAny<short>(), It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
            var started = await monthEndService.MonthEndStarted(ukprn, 1819, 2);
            started.Should().BeFalse();
        }

        [Test]
        public async Task  ReturnValidMonthEndJobIdFromCache()
        {
            mocker.Mock<IMonthEndCache>()
                .Setup(cache => cache.GetMonthEndDetails(It.Is<long>(expectedUkprn => expectedUkprn == ukprn),
                    It.IsAny<short>(),
                    It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new MonthEndDetails
                {
                    JobId = jobId
                }));
                
          var actual =  await monthEndService.GetMonthEndJobId(ukprn,1819, 2);
            actual.Should().Be(jobId);
        }

    }
}
