using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
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

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();

            mocker.Mock<IPaymentLogger>();

            paymentHistoryRepository = mocker.Mock<IPaymentHistoryRepository>();

            sut = mocker.Create<ClawbackRemovedLearnerAimPaymentsProcessor>();

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
                CollectionPeriod = new CollectionPeriod { AcademicYear = 2021, Period = 1 },
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
                It.IsAny<short>(),
                It.IsAny<byte>(),
                It.IsAny<byte>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PaymentModel>());

            var listOfCalculatedRequiredLevyAmounts = await sut.GenerateClawbackForRemovedLearnerAim(message, CancellationToken.None);

            listOfCalculatedRequiredLevyAmounts.Count.Should().Be(0);
        }

    }
}