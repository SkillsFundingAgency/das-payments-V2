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
using SFA.DAS.Payments.Model.Core.OnProgramme;
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
        private Mock<IPaymentClawbackRepository> paymentClawbackRepository;
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

            mocker.Mock<IPaymentLogger>();

            paymentClawbackRepository = mocker.Mock<IPaymentClawbackRepository>();

            sut = mocker.Create<ClawbackRemovedLearnerAimsProcessor>(
                new NamedParameter("mapper", mapper),
                new AutowiringParameter());
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
        }

        [Test]
        public async Task GivenClawbackGeneratedThenRequiredLevyAmountEventIsMappedCorrectlyFromPaymentModel()
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
                Ukprn = 19,
                FundingPlatformType = FundingPlatformType.SubmitLearnerData
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
                FundingPlatformType = FundingPlatformType.SubmitLearnerData
            };

            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(1);

            AssertCalculatedRequiredLevyAmountEvent(listOfCalculatedRequiredLevyAmounts.First(), historicalPayment, message);

            paymentClawbackRepository.Verify(r =>
                r.SaveClawbackPayments(It.Is<IEnumerable<PaymentModel>>(p =>
                AssertPaymentModel(p.Single(), message, originalEventId, originalEventTime)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GivenClawbackGeneratedThenRequiredLevyAmountEventIsMappedCorrectlyFromPaymentModeWhenFundingPlatformTypeNotPopulated()
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
                Ukprn = 19,
                FundingPlatformType = null
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
                FundingPlatformType = null
            };

            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(1);

            AssertCalculatedRequiredLevyAmountEvent(listOfCalculatedRequiredLevyAmounts.First(), historicalPayment, message);

            paymentClawbackRepository.Verify(r =>
                r.SaveClawbackPayments(It.Is<IEnumerable<PaymentModel>>(p =>
                AssertPaymentModel(p.Single(), message, originalEventId, originalEventTime)), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GivenNoHistoricalPaymentThenNoClawbackGenerated()
        {
            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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

            paymentClawbackRepository.Verify(r => r.SaveClawbackPayments(It.IsAny<IEnumerable<PaymentModel>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsZeroThenNoClawbackGenerated()
        {
            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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
                    },
                    new PaymentModel
                    {
                        Amount = -100,
                        FundingSource = FundingSourceType.Levy,
                        ContractType = ContractType.Act1,
                    }
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);

            paymentClawbackRepository.Verify(r => r.SaveClawbackPayments(It.IsAny<IEnumerable<PaymentModel>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsNotZeroThenClawbackGeneratedAndCalculatedRequiredLevyAmountEventIsOnlyGeneratedForLevyAndTransferPayment()
        {
            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.Levy,
                        ContractType = ContractType.Act1,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.Transfer,
                        ContractType = ContractType.Act1,
                    },
                    new PaymentModel
                    {
                        Amount = 100,
                        EventId = Guid.NewGuid(),
                        JobId = 10,
                        ClawbackSourcePaymentEventId = null,
                        CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 2 },
                        IlrSubmissionDateTime = DateTime.Today,
                        FundingSource = FundingSourceType.CoInvestedEmployer,
                        ContractType = ContractType.Act1,
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
                        ContractType = ContractType.Act1,
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
                    },
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(2);

            //Saving Act1 Payments
            paymentClawbackRepository.Verify(r =>
                r.SaveClawbackPayments(It.Is<IEnumerable<PaymentModel>>(models => models.Count() == 3),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GivenSumOfHistoricalPaymentIsNotZeroThenClawbackGeneratedAndCalculatedRequiredLevyAmountEventIsNotGeneratedForAct2Payment()
        {
            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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
                    },
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);

            paymentClawbackRepository.Verify(r =>
                r.SaveClawbackPayments(It.Is<IEnumerable<PaymentModel>>(models => models.Count() == 3),
                    It.IsAny<CancellationToken>()), Times.Once);
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
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
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
                    IlrSubmissionDateTime = DateTime.Today,
                    FundingSource = FundingSourceType.Levy,
                    ContractType = ContractType.Act1,
                });
            }

            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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

            paymentClawbackRepository.Verify(r => r.SaveClawbackPayments(It.IsAny<IEnumerable<PaymentModel>>(), It.IsAny<CancellationToken>()), Times.Once);
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
                }
            };


            paymentClawbackRepository.Setup(x => x.GetReadOnlyLearnerPaymentHistory(
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

            paymentClawbackRepository.Verify(r => r.SaveClawbackPayments(It.IsAny<IEnumerable<PaymentModel>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        private void AssertCalculatedRequiredLevyAmount(int index, int newAmount, IList<CalculatedRequiredLevyAmount> listOfCalculatedRequiredLevyAmounts)
        {
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).AmountDue.Should().Be(newAmount * -1);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).JobId.Should().Be(message.JobId);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.AcademicYear.Should().Be(message.CollectionPeriod.AcademicYear);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.Period.Should().Be(message.CollectionPeriod.Period);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).IlrSubmissionDateTime.Should().Be(message.IlrSubmissionDateTime);
        }
        
        private static void AssertCalculatedRequiredLevyAmountEvent(CalculatedRequiredLevyAmount listOfCalculatedRequiredLevyAmounts, PaymentModel historicalPayment, IdentifiedRemovedLearningAim message)
        {
            listOfCalculatedRequiredLevyAmounts.AgreementId.Should().Be(historicalPayment.AgreementId);
            listOfCalculatedRequiredLevyAmounts.AgreedOnDate.Should().Be(null);
            listOfCalculatedRequiredLevyAmounts.AccountId.Should().Be(historicalPayment.AccountId);
            listOfCalculatedRequiredLevyAmounts.ActualEndDate.Should().Be(historicalPayment.ActualEndDate);
            listOfCalculatedRequiredLevyAmounts.AmountDue.Should().Be(-100);
            listOfCalculatedRequiredLevyAmounts.ApprenticeshipEmployerType.Should().Be(historicalPayment.ApprenticeshipEmployerType);
            listOfCalculatedRequiredLevyAmounts.ApprenticeshipId.Should().Be(historicalPayment.ApprenticeshipId);
            listOfCalculatedRequiredLevyAmounts.ApprenticeshipPriceEpisodeId.Should().Be(historicalPayment.ApprenticeshipPriceEpisodeId);
            listOfCalculatedRequiredLevyAmounts.CollectionPeriod.Period.Should().Be(message.CollectionPeriod.Period);
            listOfCalculatedRequiredLevyAmounts.CollectionPeriod.AcademicYear.Should().Be(message.CollectionPeriod.AcademicYear);
            listOfCalculatedRequiredLevyAmounts.CompletionAmount.Should().Be(historicalPayment.CompletionAmount);
            listOfCalculatedRequiredLevyAmounts.CompletionStatus.Should().Be(historicalPayment.CompletionStatus);
            listOfCalculatedRequiredLevyAmounts.ContractType.Should().Be(historicalPayment.ContractType);
            listOfCalculatedRequiredLevyAmounts.DeliveryPeriod.Should().Be(historicalPayment.DeliveryPeriod);
            listOfCalculatedRequiredLevyAmounts.EarningEventId.Should().Be(Guid.Empty);
            listOfCalculatedRequiredLevyAmounts.EventTime.Should().BeCloseTo(DateTimeOffset.UtcNow, 100);
            listOfCalculatedRequiredLevyAmounts.IlrFileName.Should().Be(null);
            listOfCalculatedRequiredLevyAmounts.IlrSubmissionDateTime.Should().Be(message.IlrSubmissionDateTime);
            listOfCalculatedRequiredLevyAmounts.InstalmentAmount.Should().Be(historicalPayment.InstalmentAmount);
            listOfCalculatedRequiredLevyAmounts.JobId.Should().Be(message.JobId);
            listOfCalculatedRequiredLevyAmounts.Learner.ReferenceNumber.Should().Be(historicalPayment.LearnerReferenceNumber);
            listOfCalculatedRequiredLevyAmounts.Learner.Uln.Should().Be(historicalPayment.LearnerUln);
            listOfCalculatedRequiredLevyAmounts.LearningAim.FrameworkCode.Should().Be(historicalPayment.LearningAimFrameworkCode);
            listOfCalculatedRequiredLevyAmounts.LearningAim.FundingLineType.Should().Be(historicalPayment.LearningAimFundingLineType);
            listOfCalculatedRequiredLevyAmounts.LearningAim.PathwayCode.Should().Be(historicalPayment.LearningAimPathwayCode);
            listOfCalculatedRequiredLevyAmounts.LearningAim.ProgrammeType.Should().Be(historicalPayment.LearningAimProgrammeType);
            listOfCalculatedRequiredLevyAmounts.LearningAim.Reference.Should().Be(historicalPayment.LearningAimReference);
            listOfCalculatedRequiredLevyAmounts.LearningAim.SequenceNumber.Should().Be(0);
            listOfCalculatedRequiredLevyAmounts.LearningAim.StandardCode.Should().Be(historicalPayment.LearningAimStandardCode);
            listOfCalculatedRequiredLevyAmounts.LearningAim.StartDate.Should().Be(historicalPayment.StartDate);
            listOfCalculatedRequiredLevyAmounts.LearningStartDate.Should().Be(historicalPayment.LearningStartDate);
            listOfCalculatedRequiredLevyAmounts.NumberOfInstalments.Should().Be(historicalPayment.NumberOfInstalments);
            listOfCalculatedRequiredLevyAmounts.OnProgrammeEarningType.Should().Be((OnProgrammeEarningType)historicalPayment.TransactionType);
            listOfCalculatedRequiredLevyAmounts.PlannedEndDate.Should().Be(historicalPayment.PlannedEndDate);
            listOfCalculatedRequiredLevyAmounts.PriceEpisodeIdentifier.Should().Be(historicalPayment.PriceEpisodeIdentifier);
            listOfCalculatedRequiredLevyAmounts.Priority.Should().Be(0);
            listOfCalculatedRequiredLevyAmounts.ReportingAimFundingLineType.Should().Be(historicalPayment.ReportingAimFundingLineType);
            listOfCalculatedRequiredLevyAmounts.SfaContributionPercentage.Should().Be(historicalPayment.SfaContributionPercentage);
            listOfCalculatedRequiredLevyAmounts.StartDate.Should().Be(historicalPayment.StartDate);
            listOfCalculatedRequiredLevyAmounts.TransactionType.Should().Be(historicalPayment.TransactionType);
            listOfCalculatedRequiredLevyAmounts.TransferSenderAccountId.Should().Be(historicalPayment.TransferSenderAccountId);
            listOfCalculatedRequiredLevyAmounts.Ukprn.Should().Be(historicalPayment.Ukprn);
            listOfCalculatedRequiredLevyAmounts.FundingPlatformType.Should().Be(historicalPayment.FundingPlatformType);
        }

        private static bool AssertPaymentModel(PaymentModel actualPayment, IdentifiedRemovedLearningAim message, Guid originalEventId, DateTimeOffset originalEventTime)
        {
            return actualPayment.Id == 0
                   && actualPayment.Amount == -100
                   && actualPayment.JobId == message.JobId
                   && actualPayment.ClawbackSourcePaymentEventId == originalEventId
                   && actualPayment.CollectionPeriod.Period == message.CollectionPeriod.Period
                   && actualPayment.CollectionPeriod.AcademicYear == message.CollectionPeriod.AcademicYear
                   && actualPayment.IlrSubmissionDateTime == message.IlrSubmissionDateTime
                   && actualPayment.EventId != originalEventId
                   && actualPayment.EventTime != originalEventTime
                   && actualPayment.RequiredPaymentEventId == Guid.Empty
                   && actualPayment.EarningEventId == Guid.Empty
                   && actualPayment.FundingSourceEventId == Guid.Empty
                   && actualPayment.FundingPlatformType == FundingPlatformType.SubmitLearnerData;
        }
    }
}