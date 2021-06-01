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
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class ClawbackRemovedLearnerAimPaymentsProcessorTests
    {
        private AutoMock mocker;

        private ClawbackRemovedLearnerAimPaymentsProcessor sut;
        private Mock<IPaymentHistoryRepository> paymentHistoryRepository;
        private IdentifiedRemovedLearningAim message;

        private Mapper mapper;
        private PaymentModel historicalPayment;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            mapper = new Mapper(config);
        }

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            mocker.Mock<IPaymentLogger>();

            paymentHistoryRepository = mocker.Mock<IPaymentHistoryRepository>();

            sut = mocker.Create<ClawbackRemovedLearnerAimPaymentsProcessor>(
                new NamedParameter("mapper", mapper),
                new AutowiringParameter());

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

            historicalPayment = new PaymentModel
            {
                AccountId = 1,
                ActualEndDate = DateTime.Today,
                AgreementId = "AgreementId",
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                Amount = 0,
                ApprenticeshipId = 2,
                ApprenticeshipPriceEpisodeId = 3,
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 4 },
                ClawbackSourcePaymentId = null,
                CompletionAmount = 5,
                CompletionStatus = 6,
                ContractType = ContractType.Act1,
                DeliveryPeriod = 7,
                EarningEventId = Guid.NewGuid(),
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.Now,
                FundingSource = FundingSourceType.Levy,
                FundingSourceEventId = Guid.NewGuid(),
                Id = 8,
                IlrSubmissionDateTime = DateTime.Today,
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
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
        }

        [Test]
        public async Task GivenNoHistoricalPaymentThenNoRefundGenerated()
        {
            paymentHistoryRepository.Setup(x => x.GetPaymentHistoryForClawback(
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
        public async Task GivenSumOfHistoricalPaymentIsZeroThenNoRefundGenerated()
        {
            paymentHistoryRepository.Setup(x => x.GetPaymentHistoryForClawback(
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
                        Amount = 100
                    },
                    new PaymentModel
                    {
                        Amount = -100
                    }
                });

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);
        }

        [TestCase(100, 200)]
        [TestCase(-100, -200)]
        [TestCase(100, -200)]
        [TestCase(200, -100)]
        [TestCase(200, null)]
        [TestCase(-200, null)]
        public async Task GivenAllHistoricalPaymentsDoesNotHaveSourcePaymentIdAndSumOfAllPaymentIsNotZeroThenReversalGeneratedForThosePaymentRecords(int firstPaymentAmount, int? secondPaymentAmount)
        {

            var historicalPayments = new List<PaymentModel>
            {
                new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    Amount = firstPaymentAmount,
                    JobId = 10,
                    ClawbackSourcePaymentId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    IlrSubmissionDateTime = DateTime.Today,
                }
            };

            if (secondPaymentAmount.HasValue)
            {
                historicalPayments.Add(new PaymentModel
                {
                    EventId = Guid.NewGuid(),
                    Amount = secondPaymentAmount.Value,
                    JobId = 11,
                    ClawbackSourcePaymentId = null,
                    CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
                    IlrSubmissionDateTime = DateTime.Today,
                });
            }

            paymentHistoryRepository.Setup(x => x.GetPaymentHistoryForClawback(
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

        private void AssertCalculatedRequiredLevyAmount(int index, int newAmount, IReadOnlyCollection<CalculatedRequiredLevyAmount> listOfCalculatedRequiredLevyAmounts)
        {
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).AmountDue.Should().Be(newAmount * -1);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).JobId.Should().Be(message.JobId);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.AcademicYear.Should().Be(message.CollectionPeriod.AcademicYear);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).CollectionPeriod.Period.Should().Be(message.CollectionPeriod.Period);
            listOfCalculatedRequiredLevyAmounts.ElementAt(index).IlrSubmissionDateTime.Should().Be(message.IlrSubmissionDateTime);
        }
    }
}