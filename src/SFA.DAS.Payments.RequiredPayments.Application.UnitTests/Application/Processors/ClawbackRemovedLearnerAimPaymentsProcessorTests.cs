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
    public class ClawbackRemovedLearnerAimPaymentsProcessorTests
    {
        private AutoMock mocker;

        private ClawbackRemovedLearnerAimPaymentsProcessor sut;
        private Mock<IPaymentClawbackRepository> paymentClawbackRepository;
        private IdentifiedRemovedLearningAim message;

        private Mapper mapper;

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

            paymentClawbackRepository = mocker.Mock<IPaymentClawbackRepository>();

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
        }

        [TearDown]
        public void TearDown()
        {
            mocker.Dispose();
        }

        [Test]
        public async Task GivenNoHistoricalPaymentThenNoClawbackGenerated()
        {
            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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
            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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
            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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
            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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

            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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


            paymentClawbackRepository.Setup(x => x.GetLearnerPaymentHistory(
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
    }
}