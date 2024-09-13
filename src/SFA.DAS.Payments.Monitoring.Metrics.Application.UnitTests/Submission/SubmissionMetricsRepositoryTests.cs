using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Application.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.Submission
{
    [TestFixture]
    public class SubmissionMetricsRepositoryTests
    {
        private Mock<IMetricsQueryDataContextFactory> _dataContextFactory;
        private InMemoryMetricsQueryDataContext _dataContext;
        private Mock<IMetricsPersistenceDataContext> _dataPersistence;
        private Mock<IPaymentLogger> _logger;
        private SubmissionMetricsRepository _sut;
        private Fixture _fixture;
        private long _jobId;
        private long _ukprn;
        private short _academicYear;
        private byte _currentCollectionPeriod;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _dataContext = new InMemoryMetricsQueryDataContext();
            _dataContextFactory = new Mock<IMetricsQueryDataContextFactory>();
            _dataContextFactory.Setup(x => x.Create()).Returns(_dataContext);

            _dataPersistence = new Mock<IMetricsPersistenceDataContext>();

            _logger = new Mock<IPaymentLogger>();

            _jobId = _fixture.Create<long>();
            _ukprn = _fixture.Create<long>();
            _academicYear = 2324;
            _currentCollectionPeriod = 12;

            _sut = new SubmissionMetricsRepository(_dataPersistence.Object, _dataContextFactory.Object, _logger.Object);
        }

        [Test]
        public async Task GetAlreadyPaidDataLockedEarnings_Should_Only_Include_SLD_Platform_Metrics()
        {
            var expectedAlreadyPaidDataLockedEarningsTotal = SetupAlreadyPaidDataLockedEarnings();
            
            var alreadyPaidDataLockedEarningsTotal = await _sut.GetAlreadyPaidDataLockedEarnings(_ukprn, _jobId, new CancellationToken());

            alreadyPaidDataLockedEarningsTotal.Should().Be(expectedAlreadyPaidDataLockedEarningsTotal);
        }
        
        [TestCase(FundingSourceType.CoInvestedEmployer)]
        [TestCase(FundingSourceType.CoInvestedSfa)]
        [TestCase(FundingSourceType.FullyFundedSfa)]
        public async Task GetRequiredPayments_Should_Only_Include_SLD_Platform_Metrics(FundingSourceType fundingSourceType)
        {
            var expectedLearningTransactionTypeTotal = SetupRequiredAndClawedBackPayments(fundingSourceType);
            
            var requiredPayments = await _sut.GetRequiredPayments(_ukprn, _jobId, new CancellationToken());

            requiredPayments[0].TransactionType1.Should().Be(expectedLearningTransactionTypeTotal);
        }
        
        [Test]
        public async Task GetYearToDatePaymentsTotal_Should_Only_Include_SLD_Platform_Metrics()
        {
            var expectedTotal = SetupYearToDatePayments();

            var yearToDatePaymentsTotal = await _sut.GetYearToDatePaymentsTotal(_ukprn, _academicYear, _currentCollectionPeriod, new CancellationToken());

            yearToDatePaymentsTotal.ContractType1.Should().Be(expectedTotal);
            yearToDatePaymentsTotal.ContractType2.Should().Be(0);
        }

        private decimal SetupYearToDatePayments()
        {
            var historicalPayments = new List<PaymentModel>();

            for(byte period = 1; period <= (_currentCollectionPeriod - 1); period++)
            {
                var sldPayment = CreateHistoricalPayment(period, FundingPlatformType.SubmitLearnerData);

                var dasPayment = CreateHistoricalPayment(period, FundingPlatformType.DigitalApprenticeshipService);

                historicalPayments.Add(sldPayment);
                historicalPayments.Add(dasPayment);
            }

            _dataContext.Payments.AddRange(historicalPayments);
            _dataContext.SaveChanges();

            return historicalPayments.Where(x => x.FundingPlatformType == FundingPlatformType.SubmitLearnerData)
                .Sum(x => x.Amount);
        }

        private PaymentModel CreateHistoricalPayment(byte period, FundingPlatformType fundingPlatformType)
        {
            var historicalPayment = CreateDefaultPaymentValues();
            historicalPayment.FundingPlatformType = fundingPlatformType;
            historicalPayment.CollectionPeriod = new CollectionPeriod { AcademicYear = _academicYear, Period = period };

            return historicalPayment;
        }
        
        private decimal SetupRequiredAndClawedBackPayments(FundingSourceType fundingSourceType)
        {
            decimal totalAmountSld = 0m;
            var clawbackPayments = new List<PaymentModel>();

            for (byte period = 1; period <= (_currentCollectionPeriod - 1); period++)
            {
                var sldPayment = CreateClawedBackPayment(period, FundingPlatformType.SubmitLearnerData, Guid.NewGuid(), fundingSourceType);

                var dasPayment = CreateClawedBackPayment(period, FundingPlatformType.DigitalApprenticeshipService, Guid.NewGuid(), fundingSourceType);

                clawbackPayments.Add(sldPayment);
                clawbackPayments.Add(dasPayment);

                totalAmountSld += sldPayment.Amount;
            }
            
            var requiredPaymentEvents = new List<RequiredPaymentEventModel>();

            foreach (var payment in clawbackPayments.Where(x => x.FundingPlatformType == FundingPlatformType.SubmitLearnerData))
            {
                var requiredPayment = CreateRequiredPayment(payment.Id, _currentCollectionPeriod, payment.ClawbackSourcePaymentEventId.Value);
                requiredPaymentEvents.Add(requiredPayment);
            }
            
            _dataContext.Payments.AddRange(clawbackPayments);
            _dataContext.RequiredPaymentEvents.AddRange(requiredPaymentEvents);
            _dataContext.SaveChanges();

            totalAmountSld += requiredPaymentEvents.Sum(x => x.Amount);

            return totalAmountSld;
        }

        private PaymentModel CreateClawedBackPayment(byte period, FundingPlatformType fundingPlatformType, Guid clawbackSourcePaymentEventId, FundingSourceType fundingSourceType)
        {
            var clawedBackPayment = CreateDefaultPaymentValues();
            clawedBackPayment.Id = Convert.ToInt32($"{(int)fundingPlatformType}{period}");
            clawedBackPayment.FundingPlatformType = fundingPlatformType;
            clawedBackPayment.CollectionPeriod = new CollectionPeriod { AcademicYear = _academicYear, Period = period };
            clawedBackPayment.ClawbackSourcePaymentEventId = clawbackSourcePaymentEventId;
            clawedBackPayment.FundingSource = fundingSourceType;
            return clawedBackPayment;
        }

        private RequiredPaymentEventModel CreateRequiredPayment(long id, byte period, Guid clawbackSourcePaymentEventId)
        {
            var requiredPayment = new RequiredPaymentEventModel
            {
                Ukprn = _ukprn,
                JobId = _jobId,
                ClawbackSourcePaymentEventId = clawbackSourcePaymentEventId,
                Id = 1000 + id,
                EventId = Guid.NewGuid(),
                EarningEventId = Guid.NewGuid(),
                PriceEpisodeIdentifier = _fixture.Create<string>(),
                ContractType = ContractType.Act1,
                TransactionType = TransactionType.Learning,
                SfaContributionPercentage = 1,
                Amount = _fixture.Create<decimal>(),
                CollectionPeriod = new CollectionPeriod { AcademicYear = _academicYear, Period = period },
                DeliveryPeriod = _fixture.Create<byte>(),
                LearnerReferenceNumber = _fixture.Create<string>(),
                LearningAimProgrammeType = _fixture.Create<int>(),
                LearningAimStandardCode = _fixture.Create<int>(),
                LearningAimFrameworkCode = _fixture.Create<int>(),
                LearningAimPathwayCode = _fixture.Create<int>(),
                LearningAimFundingLineType = _fixture.Create<string>(),
                AgreementId = _fixture.Create<string>(),
                IlrSubmissionDateTime = DateTime.Now,
                EventTime = new DateTimeOffset(DateTime.Now),
                AccountId = _fixture.Create<long>(),
                StartDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddYears(2),
                CompletionStatus = 0,
                CompletionAmount = _fixture.Create<decimal>(),
                InstalmentAmount = _fixture.Create<decimal>(),
                NumberOfInstalments = _fixture.Create<short>(),
                LearningStartDate = DateTime.Now.AddDays(30),
                ApprenticeshipId = _fixture.Create<long>(),
                ApprenticeshipPriceEpisodeId = _fixture.Create<long>(),
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                EventType = _fixture.Create<string>(),
                AgeAtStartOfLearning = _fixture.Create<byte>()
            };

            return requiredPayment;
        }
        
        private decimal SetupAlreadyPaidDataLockedEarnings()
        {
            var payments = new List<PaymentModel>();

            var sldPayment = CreateDefaultPaymentValues();
            sldPayment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
            sldPayment.CollectionPeriod = new CollectionPeriod { AcademicYear = _academicYear, Period = 1 };

            payments.Add(sldPayment);

            var dasPayment = CreateDefaultPaymentValues();
            dasPayment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
            dasPayment.CollectionPeriod = new CollectionPeriod { AcademicYear = _academicYear, Period = 2 };

            payments.Add(dasPayment);
            
            var sldEventId = Guid.NewGuid();
            var dataLockedSldPayment = payments.First(x => x.FundingPlatformType == FundingPlatformType.SubmitLearnerData);
            var dasEventId = Guid.NewGuid();
            var dataLockedDasPayment = payments.First(x => x.FundingPlatformType == FundingPlatformType.DigitalApprenticeshipService);

            var sldDataLockEvent = CreateDataLockEventForPayment(sldEventId, dataLockedSldPayment);
            var dasDataLockEvent = CreateDataLockEventForPayment(dasEventId, dataLockedDasPayment);

            _dataContext.Payments.AddRange(payments);
            _dataContext.DataLockEvent.Add(sldDataLockEvent);
            _dataContext.DataLockEvent.Add(dasDataLockEvent);
            _dataContext.SaveChanges();

            return dataLockedSldPayment.Amount;
        }

        private DataLockEventModel CreateDataLockEventForPayment(Guid eventId, PaymentModel payment)
        {
            var dataLockEvent = new DataLockEventModel
            {
                EarningEventId = Guid.NewGuid(),
                ContractType = ContractType.Act1,
                AgreementId = _fixture.Create<string>(),
                PriceEpisodes = new List<DataLockEventPriceEpisodeModel>(),
                NonPayablePeriods = new List<DataLockEventNonPayablePeriodModel>
                {
                    new DataLockEventNonPayablePeriodModel
                    {
                        DataLockEventId = eventId,
                        DataLockEventNonPayablePeriodId = Guid.NewGuid(),
                        PriceEpisodeIdentifier = _fixture.Create<string>(),
                        TransactionType = payment.TransactionType,
                        AcademicYear = _academicYear,
                        CollectionPeriod = _currentCollectionPeriod,
                        DeliveryPeriod = payment.DeliveryPeriod,
                        Amount = _fixture.Create<decimal>(),
                        SfaContributionPercentage = 1,
                        LearningStartDate = DateTime.Now.AddMonths(1),
                        Failures = new EditableList<DataLockEventNonPayablePeriodFailureModel>()
                    }
                },
                PayablePeriods = new EditableList<DataLockEventPayablePeriodModel>(),
                IlrFileName = _fixture.Create<string>(),
                SfaContributionPercentage = 1,
                EventType = _fixture.Create<string>(),
                IsPayable = false,
                DataLockSource = DataLockSource.Submission,
                AgeAtStartOfLearning = _fixture.Create<int>(),
                EventId = eventId,
                AcademicYear = _academicYear,
                StartDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddYears(2),
                CompletionStatus = 0,
                CompletionAmount = _fixture.Create<decimal>(),
                InstalmentAmount = _fixture.Create<decimal>(),
                NumberOfInstalments = _fixture.Create<short>(),
                LearningStartDate = DateTime.Now.AddMonths(1),
                CollectionPeriod = _currentCollectionPeriod,
                LearnerReferenceNumber = payment.LearnerReferenceNumber,
                LearnerUln = _fixture.Create<long>(),
                LearningAimReference = payment.LearningAimReference,
                LearningAimProgrammeType = payment.LearningAimProgrammeType,
                LearningAimStandardCode = payment.LearningAimStandardCode,
                LearningAimFrameworkCode = payment.LearningAimFrameworkCode,
                LearningAimPathwayCode = payment.LearningAimPathwayCode,
                LearningAimFundingLineType = _fixture.Create<string>(),
                Ukprn = _ukprn,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = _jobId,
                LearningAimSequenceNumber = _fixture.Create<long>()
            };
            return dataLockEvent;
        }

        private PaymentModel CreateDefaultPaymentValues()
        {
            return new PaymentModel
            {
                Ukprn = _ukprn,
                JobId = _jobId,
                ContractType = ContractType.Act1,
                Amount = _fixture.Create<decimal>(),
                EventId = Guid.NewGuid(),
                EarningEventId = Guid.NewGuid(),
                FundingSourceEventId = Guid.NewGuid(),
                RequiredPaymentEventId = Guid.NewGuid(),
                ClawbackSourcePaymentEventId = Guid.Empty,
                EventTime = new DateTimeOffset(DateTime.Now),
                LearnerReferenceNumber = _fixture.Create<string>(),
                LearnerUln = _fixture.Create<long>(),
                PriceEpisodeIdentifier = _fixture.Create<string>(),
                DeliveryPeriod = _fixture.Create<byte>(),
                LearningAimReference = _fixture.Create<string>(),
                LearningAimProgrammeType = _fixture.Create<int>(),
                LearningAimStandardCode = _fixture.Create<int>(),
                LearningAimFrameworkCode = _fixture.Create<int>(),
                LearningAimPathwayCode = _fixture.Create<int>(),
                LearningAimFundingLineType = _fixture.Create<string>(),
                TransactionType = TransactionType.Learning,
                FundingSource = FundingSourceType.Levy,
                IlrSubmissionDateTime = DateTime.Now,
                SfaContributionPercentage = 1,
                AccountId = _fixture.Create<long>(),
                StartDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddYears(2),
                CompletionStatus = 0,
                CompletionAmount = _fixture.Create<decimal>(),
                InstalmentAmount = _fixture.Create<decimal>(),
                NumberOfInstalments = _fixture.Create<short>(),
                AgreementId = _fixture.Create<string>(),
                LearningStartDate = DateTime.Now.AddDays(30),
                ApprenticeshipId = _fixture.Create<long>(),
                ApprenticeshipPriceEpisodeId = _fixture.Create<long>(),
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ReportingAimFundingLineType = _fixture.Create<string>(),
                LearningAimSequenceNumber = _fixture.Create<long>(),
                AgeAtStartOfLearning = _fixture.Create<byte>()
            };
        }
    }
}
