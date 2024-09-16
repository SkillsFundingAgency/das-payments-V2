using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Factories;
using SFA.DAS.Payments.Monitoring.Metrics.Application.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Data;

namespace SFA.DAS.Payments.Monitoring.Metrics.Application.UnitTests.PeriodEnd
{
    [TestFixture]
    public class PeriodEndMetricsRepositoryTests
    {
        private Mock<IMetricsQueryDataContextFactory> _dataContextFactory;
        private InMemoryMetricsQueryDataContext _dataContext;
        private Mock<IMetricsPersistenceDataContext> _dataPersistence;
        private Mock<IPaymentLogger> _logger;
        private PeriodEndMetricsRepository _sut;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new Fixture();

            _dataContext = new InMemoryMetricsQueryDataContext();
            _dataContextFactory = new Mock<IMetricsQueryDataContextFactory>();
            _dataContextFactory.Setup(x => x.Create()).Returns(_dataContext);

            _dataPersistence = new Mock<IMetricsPersistenceDataContext>();

            _logger = new Mock<IPaymentLogger>();

            _sut = new PeriodEndMetricsRepository(_dataPersistence.Object, _dataContextFactory.Object, _logger.Object);
        }

        [Test]
        public async Task WhenCallingGetYearToDatePayments_WithMultipleProviders_TheYtdPaymentsAreCorrect()
        {
            var payments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    Ukprn = 1,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act1,
                    Amount = 1000,
                    FundingPlatformType = FundingPlatformType.SubmitLearnerData
                },
                new PaymentModel
                {
                    Ukprn = 1,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act2,
                    Amount = 2000,
                    FundingPlatformType = FundingPlatformType.SubmitLearnerData
                },

                new PaymentModel
                {
                    Ukprn = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act1,
                    Amount = 10000,
                    FundingPlatformType = FundingPlatformType.SubmitLearnerData
                },
                new PaymentModel
                {
                    Ukprn = 2,
                    CollectionPeriod = CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(2021, 1),
                    ContractType = ContractType.Act2,
                    Amount = 20000,
                    FundingPlatformType = FundingPlatformType.SubmitLearnerData
                },
            };

            await _dataContext.AddRangeAsync(payments);
            await _dataContext.SaveChangesAsync();

            var actual = await _sut.GetYearToDatePayments(2021, 2, CancellationToken.None);

            actual.Where(x => x.Ukprn == 1).Should().HaveCount(1);
            actual.Where(x => x.Ukprn == 2).Should().HaveCount(1);
            actual.Where(x => x.Ukprn == 3).Should().BeEmpty();

            actual.Single(x => x.Ukprn == 1).Total.Should().Be(3000);
            actual.Single(x => x.Ukprn == 2).Total.Should().Be(30000);
        }

        [Test]
        public async Task WhenCallingGetPeriodEndProviderDataLockTypeCounts_ThenMetricsQueryDataContextCalled()
        {
            short academicYear = 2021;
            byte collectionPeriod = 3;

            var persistenceContextMock = new Mock<IMetricsPersistenceDataContext>();
            var metricsQueryDataContextFactoryMock = new Mock<IMetricsQueryDataContextFactory>();
            var metricsQueryDataContextMock = new Mock<IMetricsQueryDataContext>();
            var loggerMock = new Mock<IPaymentLogger>();

            metricsQueryDataContextFactoryMock
                .Setup(x => x.Create())
                .Returns(metricsQueryDataContextMock.Object);

            var sut = new PeriodEndMetricsRepository(persistenceContextMock.Object,
                metricsQueryDataContextFactoryMock.Object, loggerMock.Object);

            await sut.GetPeriodEndProviderDataLockTypeCounts(academicYear, collectionPeriod,
                It.IsAny<CancellationToken>());

            metricsQueryDataContextMock.Verify(
                x => x.GetPeriodEndProviderDataLockCounts(academicYear, collectionPeriod,
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task WhenCallingGetTransactionTypesByContractType_ThenDASPlatformPaymentsAreExcluded()
        {
            var ukprn = _fixture.Create<long>();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            foreach (var enumValue in Enum.GetValues(typeof(TransactionType)))
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = Enum.Parse<TransactionType>(enumValue.ToString());
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                sldPayments.Add(payment);
            }

            foreach (var enumValue in Enum.GetValues(typeof(TransactionType)))
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = Enum.Parse<TransactionType>(enumValue.ToString());
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                dasPayments.Add(payment);
            }

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            await _dataContext.SaveChangesAsync();

            var providerTransactionTypeAmounts =
                await _sut.GetTransactionTypesByContractType(academicYear, collectionPeriod, new CancellationToken());

            var act1TransactionAmounts = providerTransactionTypeAmounts.First(x => x.ContractType == ContractType.Act1);
            act1TransactionAmounts.TransactionType1.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Learning).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType2.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Completion).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType3.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Balancing).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType4.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.First16To18EmployerIncentive).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType5.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.First16To18ProviderIncentive).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType6.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Second16To18EmployerIncentive).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType7.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Second16To18ProviderIncentive).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType8.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.OnProgramme16To18FrameworkUplift).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType9.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Completion16To18FrameworkUplift).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType10.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.Balancing16To18FrameworkUplift).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType11.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.FirstDisadvantagePayment).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType12.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.SecondDisadvantagePayment).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType13.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.OnProgrammeMathsAndEnglish).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType14.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.BalancingMathsAndEnglish).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType15.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.LearningSupport).Sum(x => x.Amount));
            act1TransactionAmounts.TransactionType16.Should()
                .Be(sldPayments.Where(x => x.TransactionType == TransactionType.CareLeaverApprenticePayment).Sum(x => x.Amount));
        }

        [Test]
        public async Task WhenCallingGetPaymentAmountsForNegativeEarningsLearnersByContractType_ThenDASPlatformPaymentsAreExcluded()
        {
            var ukprn = _fixture.Create<long>();
            var ulns = _fixture.CreateMany<long>(5).ToList();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            foreach (var uln in ulns)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.LearnerUln = uln;
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                sldPayments.Add(payment);
            }

            foreach (var uln in ulns)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.LearnerUln = uln;
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                dasPayments.Add(payment);
            }

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            await _dataContext.SaveChangesAsync();

            var learnerContractTypeAmounts = await _sut.GetPaymentAmountsForNegativeEarningsLearnersByContractType(ulns, academicYear, new CancellationToken());
            foreach (var uln in ulns)
            {
                learnerContractTypeAmounts.First(x => x.LearnerUln == uln).ContractType1.Should().Be(sldPayments.First(x => x.LearnerUln == uln).Amount);
            }
        }

        [Test]
        public async Task WhenCallingGetFundingSourceAmountsByContractType_ThenDASPlatformPaymentsAreExcluded()
        {
            var ukprn = _fixture.Create<long>();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            foreach (var enumValue in Enum.GetValues(typeof(FundingSourceType)))
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingSource = Enum.Parse<FundingSourceType>(enumValue.ToString());
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                sldPayments.Add(payment);
            }

            foreach (var enumValue in Enum.GetValues(typeof(FundingSourceType)))
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingSource = Enum.Parse<FundingSourceType>(enumValue.ToString());
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                dasPayments.Add(payment);
            }

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            await _dataContext.SaveChangesAsync();

            var providerFundingSourceAmounts = await _sut.GetFundingSourceAmountsByContractType(academicYear, collectionPeriod, new CancellationToken());

            var act1FundingSourceAmounts = providerFundingSourceAmounts.First(x => x.ContractType == ContractType.Act1);

            act1FundingSourceAmounts.FundingSource1.Should()
                .Be(sldPayments.First(x => x.FundingSource == FundingSourceType.Levy).Amount);
            act1FundingSourceAmounts.FundingSource2.Should()
                .Be(sldPayments.First(x => x.FundingSource == FundingSourceType.CoInvestedSfa).Amount);
            act1FundingSourceAmounts.FundingSource3.Should()
                .Be(sldPayments.First(x => x.FundingSource == FundingSourceType.CoInvestedEmployer).Amount);
            act1FundingSourceAmounts.FundingSource4.Should()
                .Be(sldPayments.First(x => x.FundingSource == FundingSourceType.FullyFundedSfa).Amount);
            act1FundingSourceAmounts.FundingSource5.Should()
                .Be(sldPayments.First(x => x.FundingSource == FundingSourceType.Transfer).Amount);
        }

        [TestCase("16 - 18 Apprenticeship(From May 2017) Non - Levy Contract(non - procured)")]
        [TestCase("16-18 Apprenticeship Non-Levy Contract (procured)")]
        [TestCase("16-18 Apprenticeship (Employer on App Service)")]
        public async Task WhenCallingGetAlreadyPaidDataLockedEarnings_ThenDASPlatformPaymentsAreExcludedFor16To18FundingLines(string fundingLineType)
        {
            var ukprn = _fixture.Create<long>();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            for (byte period = 1; period <= 11; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                payment.LearningAimFundingLineType = fundingLineType;
                sldPayments.Add(payment);
            }

            for (byte period = 1; period <= 11; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                payment.LearningAimFundingLineType = fundingLineType;
                dasPayments.Add(payment);
            }

            var job = new LatestSuccessfulJobModel
            {
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                DcJobId = _fixture.Create<long>(),
                JobId = _fixture.Create<long>(),
                Ukprn = ukprn
            };

            var sldEventId = Guid.NewGuid();
            var dataLockedSldPayment = sldPayments.First();
            var dasEventId = Guid.NewGuid();
            var dataLockedDasPayment = dasPayments.First();

            var sldDataLockEvent = CreateDataLockEventForPayment(sldEventId, dataLockedSldPayment, job.DcJobId, collectionPeriod);
            var dasDataLockEvent = CreateDataLockEventForPayment(dasEventId, dataLockedDasPayment, job.DcJobId, collectionPeriod);

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            _dataContext.Add(job);
            _dataContext.Add(sldDataLockEvent);
            _dataContext.Add(dasDataLockEvent);
            await _dataContext.SaveChangesAsync();

            var providerFundingLineTypeAmounts = await _sut.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod, new CancellationToken());

            var ukprnFundingLineAmounts = providerFundingLineTypeAmounts.First(x => x.Ukprn == ukprn);

            ukprnFundingLineAmounts.FundingLineType16To18Amount.Should().Be(sldPayments.First().Amount);
            ukprnFundingLineAmounts.FundingLineType19PlusAmount.Should().Be(0);
            ukprnFundingLineAmounts.Total.Should().Be(sldPayments.First().Amount);
        }

        [TestCase("19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured)")]
        [TestCase("19+ Apprenticeship Non-Levy Contract (procured)")]
        [TestCase("19+ Apprenticeship (Employer on App Service)")]
        public async Task WhenCallingGetAlreadyPaidDataLockedEarnings_ThenDASPlatformPaymentsAreExcludedFor19PlusFundingLines(string fundingLineType)
        {
            var ukprn = _fixture.Create<long>();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            for (byte period = 1; period <= 11; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                payment.LearningAimFundingLineType = fundingLineType;
                sldPayments.Add(payment);
            }

            for (byte period = 1; period <= 11; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                payment.LearningAimFundingLineType = fundingLineType;
                dasPayments.Add(payment);
            }

            var job = new LatestSuccessfulJobModel
            {
                AcademicYear = academicYear,
                CollectionPeriod = collectionPeriod,
                DcJobId = _fixture.Create<long>(),
                JobId = _fixture.Create<long>(),
                Ukprn = ukprn
            };

            var sldEventId = Guid.NewGuid();
            var dataLockedSldPayment = sldPayments.First();
            var dasEventId = Guid.NewGuid();
            var dataLockedDasPayment = dasPayments.First();

            var sldDataLockEvent = CreateDataLockEventForPayment(sldEventId, dataLockedSldPayment, job.DcJobId, collectionPeriod);
            var dasDataLockEvent = CreateDataLockEventForPayment(dasEventId, dataLockedDasPayment, job.DcJobId, collectionPeriod);

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            _dataContext.Add(job);
            _dataContext.Add(sldDataLockEvent);
            _dataContext.Add(dasDataLockEvent);
            await _dataContext.SaveChangesAsync();

            var providerFundingLineTypeAmounts = await _sut.GetAlreadyPaidDataLockedEarnings(academicYear, collectionPeriod, new CancellationToken());

            var ukprnFundingLineAmounts = providerFundingLineTypeAmounts.First(x => x.Ukprn == ukprn);

            ukprnFundingLineAmounts.FundingLineType16To18Amount.Should().Be(0);
            ukprnFundingLineAmounts.FundingLineType19PlusAmount.Should().Be(sldPayments.First().Amount);
            ukprnFundingLineAmounts.Total.Should().Be(sldPayments.First().Amount);
        }

        [Test]
        public async Task WhenCallingGetYearToDatePayments_ThenDASPlatformPaymentsAreExcluded()
        {
            var ukprn = _fixture.Create<long>();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            for(byte period = 1; period < collectionPeriod; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                sldPayments.Add(payment);
            }

            for (byte period = 1; period < collectionPeriod; period++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = period };
                payment.ContractType = ContractType.Act1;
                dasPayments.Add(payment);
            }

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            await _dataContext.SaveChangesAsync();

            var yearToDateAmounts = await _sut.GetYearToDatePayments(academicYear, collectionPeriod, new CancellationToken());

            var ukprnYearToDatePayments = yearToDateAmounts.First(x => x.Ukprn == ukprn);

            ukprnYearToDatePayments.ContractType1.Should().Be(sldPayments.Sum(x => x.Amount));
            ukprnYearToDatePayments.ContractType2.Should().Be(0);
        }

        [Test]
        public async Task WhenCallingGetInLearningCount_ThenDASPlatformPaymentsAreExcluded()
        {
            var ukprn = _fixture.Create<long>();
            var ulns = _fixture.CreateMany<long>(5).ToList();
            short academicYear = 2324;
            byte collectionPeriod = 12;
            var sldPayments = new List<PaymentModel>();
            var dasPayments = new List<PaymentModel>();

            for (var count = 1; count <= 100; count++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.SubmitLearnerData;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                sldPayments.Add(payment);
            }

            for (var count = 1; count <= 50; count++)
            {
                var payment = CreateDefaultPaymentValues(ukprn);
                payment.TransactionType = TransactionType.Learning;
                payment.FundingPlatformType = FundingPlatformType.DigitalApprenticeshipService;
                payment.CollectionPeriod = new CollectionPeriod { AcademicYear = academicYear, Period = collectionPeriod };
                payment.ContractType = ContractType.Act1;
                dasPayments.Add(payment);
            }

            _dataContext.AddRange(sldPayments);
            _dataContext.AddRange(dasPayments);
            await _dataContext.SaveChangesAsync();

            var providerInLearningTotals = await _sut.GetInLearningCount(academicYear, collectionPeriod, new CancellationToken());

            var ukprnInLearningTotal = providerInLearningTotals.First(x => x.Ukprn == ukprn);
            ukprnInLearningTotal.InLearningCount.Should().Be(sldPayments.Count);
        }

        private PaymentModel CreateDefaultPaymentValues(long ukprn)
        {
            return new PaymentModel
            {
                Ukprn = ukprn,
                JobId = _fixture.Create<long>(),
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

        private DataLockEventModel CreateDataLockEventForPayment(Guid eventId, PaymentModel payment, long jobId, byte period)
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
                        AcademicYear = payment.CollectionPeriod.AcademicYear,
                        CollectionPeriod = period,
                        DeliveryPeriod = payment.DeliveryPeriod,
                        Amount = _fixture.Create<decimal>(),
                        SfaContributionPercentage = 1,
                        LearningStartDate = DateTime.Now.AddMonths(1),
                        Failures = new List<DataLockEventNonPayablePeriodFailureModel>()
                    }
                },
                PayablePeriods = new List<DataLockEventPayablePeriodModel>(),
                IlrFileName = _fixture.Create<string>(),
                SfaContributionPercentage = 1,
                EventType = _fixture.Create<string>(),
                IsPayable = false,
                DataLockSource = DataLockSource.Submission,
                AgeAtStartOfLearning = _fixture.Create<int>(),
                EventId = eventId,
                AcademicYear = payment.CollectionPeriod.AcademicYear,
                StartDate = DateTime.Now,
                PlannedEndDate = DateTime.Now.AddYears(2),
                CompletionStatus = 0,
                CompletionAmount = _fixture.Create<decimal>(),
                InstalmentAmount = _fixture.Create<decimal>(),
                NumberOfInstalments = _fixture.Create<short>(),
                LearningStartDate = DateTime.Now.AddMonths(1),
                CollectionPeriod = period,
                LearnerReferenceNumber = payment.LearnerReferenceNumber,
                LearnerUln = _fixture.Create<long>(),
                LearningAimReference = payment.LearningAimReference,
                LearningAimProgrammeType = payment.LearningAimProgrammeType,
                LearningAimStandardCode = payment.LearningAimStandardCode,
                LearningAimFrameworkCode = payment.LearningAimFrameworkCode,
                LearningAimPathwayCode = payment.LearningAimPathwayCode,
                LearningAimFundingLineType = _fixture.Create<string>(),
                Ukprn = payment.Ukprn,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = jobId,
                LearningAimSequenceNumber = _fixture.Create<long>()
            };
            return dataLockEvent;
        }
    }
}

