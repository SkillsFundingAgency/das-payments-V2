using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.RequiredPayments.Application.Mapping;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Application.Repositories;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class ClawbackRemovedLearnerAimsProcessorTests
    {
        private AutoMock mocker;

        private ClawbackRemovedLearnerAimsProcessor sut;
        private Mock<IPaymentHistoryRepository> paymentHistoryRepository;
        private IdentifiedRemovedLearningAim message;

        private Mapper mapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);

            message = new IdentifiedRemovedLearningAim
            {
                LearningAim = new LearningAim
                {
                    Reference = "reference",
                    PathwayCode = 1,
                    FrameworkCode = 2,
                    ProgrammeType = 3,
                    StandardCode = 4,
                    FundingLineType = "fundingLineType",
                    SequenceNumber = 5,
                    StartDate = DateTime.Now,
                },
                Learner = new Learner
                {
                    ReferenceNumber = "LearnerRef",
                    Uln = 123,
                },
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 4 },
                Ukprn = 12345678,
                ContractType = ContractType.Act1,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = 456
            };
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

           var logger = mocker.Mock<IPaymentLogger>();

            paymentHistoryRepository = mocker.Mock<IPaymentHistoryRepository>();
            
            var requiredPaymentEventFactory = new RequiredPaymentEventFactory(logger.Object);

            sut = mocker.Create<ClawbackRemovedLearnerAimsProcessor>(
                new NamedParameter("mapper", mapper),
                new NamedParameter("requiredPaymentEventFactory", requiredPaymentEventFactory),
                new AutowiringParameter());
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
        }

        [Test]
        public async Task GivenClawbackGeneratedThenRequiredPaymentEventIsMappedCorrectlyFromPaymentModel()
        {
            var historicalPayment = new PaymentModel
            {
                AccountId = 1,
                ActualEndDate = DateTime.Today,
                AgreementId = "AgreementId",
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                Amount = 100,
                ApprenticeshipId = 2,
                ApprenticeshipPriceEpisodeId = 3,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                ClawbackSourcePaymentEventId = null,
                CompletionAmount = 5,
                CompletionStatus = 6,
                ContractType = ContractType.Act1,
                DeliveryPeriod = 7,
                EarningEventId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now.AddDays(-1),
                FundingSource = FundingSourceType.Levy,
                FundingSourceEventId = Guid.NewGuid(),
                Id = 8,
                IlrSubmissionDateTime = DateTime.Today.AddDays(-1),
                InstalmentAmount = 9,
                JobId = 10,
                LearnerReferenceNumber = "LearnerReferenceNumber",
                LearnerUln = 11,
                LearningAimFrameworkCode = 12,
                LearningAimFundingLineType = "LearningAimFundingLineType",
                LearningAimPathwayCode = 13,
                LearningAimProgrammeType = 14,
                LearningAimReference = "LearningAimReference",
                LearningAimStandardCode = 15,
                LearningStartDate = DateTime.Today,
                NumberOfInstalments = 16,
                PlannedEndDate = DateTime.Today,
                PriceEpisodeIdentifier = "PriceEpisodeIdentifier",
                ReportingAimFundingLineType = "ReportingAimFundingLineType",
                RequiredPaymentEventId = Guid.NewGuid(),
                StartDate = DateTime.Today,
                SfaContributionPercentage = 17,
                TransactionType = TransactionType.Learning,
                TransferSenderAccountId = 18,
                Ukprn = 19
            };

            var originalEventId = Guid.NewGuid();
            var originalEventTime = DateTimeOffset.Now.AddDays(-1);

            var historicalPayment2 = new PaymentModel
            {
                Amount = 100,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                ClawbackSourcePaymentEventId = null,
                ContractType = ContractType.Act2,
                EarningEventId = Guid.NewGuid(),
                EventId = originalEventId,
                EventTime = originalEventTime,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                FundingSourceEventId = Guid.NewGuid(),
                Id = 8,
                IlrSubmissionDateTime = DateTime.Now.AddDays(-1),
                JobId = 10,
                TransactionType = TransactionType.Learning,
            };

            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                    It.IsAny<long>(),
                    It.IsAny<ContractType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<short>(),
                    It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel> { historicalPayment, historicalPayment2 });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(2);

            AssertCalculatedRequiredLevyAmountEvent(listOfCalculatedRequiredLevyAmounts.First(), historicalPayment, message);
        }

        [Test]
        public async Task GivenNoHistoricalPaymentThenNoClawbackGenerated()
        {
            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                    It.IsAny<long>(),
                    It.IsAny<ContractType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<short>(),
                    It.IsAny<byte>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel>());

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsZeroThenNoClawbackGenerated()
        {
            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                It.IsAny<long>(),
                It.IsAny<ContractType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel>
                {
                    new PaymentModel
                    {
                        Amount = 100,
                        FundingSource = FundingSourceType.Levy,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = -100,
                        FundingSource = FundingSourceType.Levy,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    }
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsNotZeroThenClawbackGeneratedAndCalculatedRequiredLevyAmountEventIsOnlyGeneratedForLevyAndTransferPayment()
        {
            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                It.IsAny<long>(),
                It.IsAny<ContractType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel>
                {
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                        DeliveryPeriod = 1,
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.Levy,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        DeliveryPeriod = 2,
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.Transfer,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        DeliveryPeriod = 2,
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.CoInvestedEmployer,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        DeliveryPeriod = 2,
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.CoInvestedSfa,
                        ContractType = ContractType.Act1,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        DeliveryPeriod = 2,
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.FullyFundedSfa,
                        ContractType = ContractType.Act2,
                        TransactionType = TransactionType.First16To18EmployerIncentive,
                    },
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(4);
            listOfCalculatedRequiredLevyAmounts.Count(rp => rp.GetType() == typeof(CalculatedRequiredLevyAmount)).Should().Be(2);
            listOfCalculatedRequiredLevyAmounts.Count(rp => rp.GetType() == typeof(CalculatedRequiredCoInvestedAmount)).Should().Be(1);
            listOfCalculatedRequiredLevyAmounts.Count(rp => rp.GetType() == typeof(CalculatedRequiredIncentiveAmount)).Should().Be(1);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsNotZeroThenClawbackGeneratedAndCalculatedRequiredLevyAmountEventIsNotGeneratedForAct2Payment()
        {
            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                It.IsAny<long>(),
                It.IsAny<ContractType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel>
                {
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.CoInvestedEmployer,
                        ContractType = ContractType.Act2,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.CoInvestedSfa,
                        ContractType = ContractType.Act2,
                        TransactionType = TransactionType.Learning,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.FullyFundedSfa,
                        ContractType = ContractType.Act2,
                        TransactionType = TransactionType.First16To18EmployerIncentive,
                    },
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(2);
            listOfCalculatedRequiredLevyAmounts.Count(rp => rp.GetType() == typeof(CalculatedRequiredCoInvestedAmount)).Should().Be(1);
            listOfCalculatedRequiredLevyAmounts.Count(rp => rp.GetType() == typeof(CalculatedRequiredIncentiveAmount)).Should().Be(1);
        }

        [TestCase(100, 200)]
        [TestCase(-100, -200)]
        [TestCase(100, -200)]
        [TestCase(200, -100)]
        [TestCase(200, null)]
        [TestCase(-200, null)]
        public async Task GivenAllHistoricalPaymentsDoesNotHaveSourcePaymentIdAndSumOfAllPaymentIsNotZeroThenClawbackGeneratedForAllPaymentRecords(int firstPaymentAmount, int? secondPaymentAmount)
        {

            var historicalPayments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    Amount = firstPaymentAmount,
                    EventId = Guid.NewGuid(),
                    JobId = 10,
                    ClawbackSourcePaymentEventId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    DeliveryPeriod = 1,
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                    TransactionType = TransactionType.Learning,
                }
            };

            if (secondPaymentAmount.HasValue)
            {
                historicalPayments.Add(new PaymentModel
                {
                    Amount = secondPaymentAmount.Value,
                    EventId = Guid.NewGuid(),
                    JobId = 11,
                    ClawbackSourcePaymentEventId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                    DeliveryPeriod = 2,
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                    TransactionType = TransactionType.Learning,
                });
            }

            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                It.IsAny<long>(),
                It.IsAny<ContractType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => historicalPayments);

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(secondPaymentAmount.HasValue ? 2 : 1);

            AssertCalculatedRequiredLevyAmount(0, firstPaymentAmount, listOfCalculatedRequiredLevyAmounts);

            if (secondPaymentAmount.HasValue)
            {
                AssertCalculatedRequiredLevyAmount(1, secondPaymentAmount.Value, listOfCalculatedRequiredLevyAmounts);
            }
        }

        [TestCase(100)]
        [TestCase(-100)]
        public async Task GivenSomeHistoricalPaymentsHaveSourcePaymentIdAndSumOfAllPaymentIsNotZeroThenClawbackGeneratedForPaymentRecordsWithoutMatchingReversalPayment(int paymentAmount)
        {
            var paymentId = Guid.NewGuid();

            var historicalPayments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    EventId = paymentId,
                    Amount = 100,
                    JobId = 10,
                    ClawbackSourcePaymentEventId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                    TransactionType = TransactionType.Learning,
                },
                new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    Amount = -100,
                    JobId = 11,
                    ClawbackSourcePaymentEventId = paymentId,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                    TransactionType = TransactionType.Learning,
                },
                new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    Amount = paymentAmount,
                    JobId = 11,
                    ClawbackSourcePaymentEventId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 3 },
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                    TransactionType = TransactionType.Learning,
                }
            };


            paymentHistoryRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
                It.IsAny<long>(),
                It.IsAny<ContractType>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => historicalPayments);

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(1);

            AssertCalculatedRequiredLevyAmount(0, paymentAmount, listOfCalculatedRequiredLevyAmounts);
        }

        private void AssertCalculatedRequiredLevyAmount(int index, int newAmount, IList<PeriodisedRequiredPaymentEvent> listOfCalculatedRequiredLevyAmounts)
        {
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).AmountDue.Should().Be(newAmount * -1);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).JobId.Should().Be(message.JobId);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.AcademicYear.Should().Be(message.CollectionPeriod.AcademicYear);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.Period.Should().Be(message.CollectionPeriod.Period);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).IlrSubmissionDateTime.Should().Be(message.IlrSubmissionDateTime);
        }
        
        private static void AssertCalculatedRequiredLevyAmountEvent(PeriodisedRequiredPaymentEvent periodisedRequiredPaymentEvent, PaymentModel historicalPayment, IdentifiedRemovedLearningAim message)
        {
            periodisedRequiredPaymentEvent.AccountId.Should().Be(historicalPayment.AccountId);
            periodisedRequiredPaymentEvent.ActualEndDate.Should().Be(historicalPayment.ActualEndDate);
            periodisedRequiredPaymentEvent.AmountDue.Should().Be(-100);
            periodisedRequiredPaymentEvent.ApprenticeshipEmployerType.Should().Be(historicalPayment.ApprenticeshipEmployerType);
            periodisedRequiredPaymentEvent.ApprenticeshipId.Should().Be(historicalPayment.ApprenticeshipId);
            periodisedRequiredPaymentEvent.ApprenticeshipPriceEpisodeId.Should().Be(historicalPayment.ApprenticeshipPriceEpisodeId);
            periodisedRequiredPaymentEvent.CollectionPeriod.Period.Should().Be(message.CollectionPeriod.Period);
            periodisedRequiredPaymentEvent.CollectionPeriod.AcademicYear.Should().Be(message.CollectionPeriod.AcademicYear);
            periodisedRequiredPaymentEvent.CompletionAmount.Should().Be(historicalPayment.CompletionAmount);
            periodisedRequiredPaymentEvent.CompletionStatus.Should().Be(historicalPayment.CompletionStatus);
            periodisedRequiredPaymentEvent.ContractType.Should().Be(historicalPayment.ContractType);
            periodisedRequiredPaymentEvent.DeliveryPeriod.Should().Be(historicalPayment.DeliveryPeriod);
            periodisedRequiredPaymentEvent.EarningEventId.Should().Be(Guid.Empty);
            periodisedRequiredPaymentEvent.EventTime.Should().BeCloseTo(DateTimeOffset.UtcNow, 100);
            periodisedRequiredPaymentEvent.IlrFileName.Should().Be(null);
            periodisedRequiredPaymentEvent.IlrSubmissionDateTime.Should().Be(message.IlrSubmissionDateTime);
            periodisedRequiredPaymentEvent.InstalmentAmount.Should().Be(historicalPayment.InstalmentAmount);
            periodisedRequiredPaymentEvent.JobId.Should().Be(message.JobId);
            periodisedRequiredPaymentEvent.Learner.ReferenceNumber.Should().Be(historicalPayment.LearnerReferenceNumber);
            periodisedRequiredPaymentEvent.Learner.Uln.Should().Be(historicalPayment.LearnerUln);
            periodisedRequiredPaymentEvent.LearningAim.FrameworkCode.Should().Be(historicalPayment.LearningAimFrameworkCode);
            periodisedRequiredPaymentEvent.LearningAim.FundingLineType.Should().Be(historicalPayment.LearningAimFundingLineType);
            periodisedRequiredPaymentEvent.LearningAim.PathwayCode.Should().Be(historicalPayment.LearningAimPathwayCode);
            periodisedRequiredPaymentEvent.LearningAim.ProgrammeType.Should().Be(historicalPayment.LearningAimProgrammeType);
            periodisedRequiredPaymentEvent.LearningAim.Reference.Should().Be(historicalPayment.LearningAimReference);
            periodisedRequiredPaymentEvent.LearningAim.SequenceNumber.Should().Be(0);
            periodisedRequiredPaymentEvent.LearningAim.StandardCode.Should().Be(historicalPayment.LearningAimStandardCode);
            periodisedRequiredPaymentEvent.LearningAim.StartDate.Should().Be(historicalPayment.StartDate);
            periodisedRequiredPaymentEvent.LearningStartDate.Should().Be(historicalPayment.LearningStartDate);
            periodisedRequiredPaymentEvent.NumberOfInstalments.Should().Be(historicalPayment.NumberOfInstalments);
            periodisedRequiredPaymentEvent.PlannedEndDate.Should().Be(historicalPayment.PlannedEndDate);
            periodisedRequiredPaymentEvent.PriceEpisodeIdentifier.Should().Be(historicalPayment.PriceEpisodeIdentifier);
            periodisedRequiredPaymentEvent.ReportingAimFundingLineType.Should().Be(historicalPayment.ReportingAimFundingLineType);
            periodisedRequiredPaymentEvent.StartDate.Should().Be(historicalPayment.StartDate);
            periodisedRequiredPaymentEvent.TransactionType.Should().Be(historicalPayment.TransactionType);
            periodisedRequiredPaymentEvent.TransferSenderAccountId.Should().Be(historicalPayment.TransferSenderAccountId);
            periodisedRequiredPaymentEvent.Ukprn.Should().Be(historicalPayment.Ukprn);
        }
    }
}