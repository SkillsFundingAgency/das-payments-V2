using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.RequiredPayments.Application.Processors;
using SFA.DAS.Payments.RequiredPayments.Domain;
using SFA.DAS.Payments.RequiredPayments.Domain.Entities;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.Application.Processors
{
    [TestFixture]
    public class RefundRemovedLearningAimProcessorTests
    {
        private AutoMock mocker;
        private List<PaymentHistoryEntity> history;

        [SetUp]
        public void SetUp()
        {
            mocker = AutoMock.GetLoose();
            history = new List<PaymentHistoryEntity>();
        }

        [Test]
        public async Task Refunds_Required_Levy_Payments()
        {
            var identifiedLearner = new IdentifiedRemovedLearningAim
            {
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                IlrSubmissionDateTime = DateTime.Now,
                JobId = 1,
                Learner = new Learner
                {
                    ReferenceNumber = "12345",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learner-ref",
                    StandardCode = 6
                },
                Ukprn = 7
            };
            history.Add(new PaymentHistoryEntity{ Amount  = 10});
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));
            mocker.Mock<IRefundRemovedLearningAimService>()
                .Setup(x => x.RefundLearningAim(It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>
                {
                    new RequiredPayment{Amount = 10, SfaContributionPercentage = .95M, EarningType = EarningType.Levy, PriceEpisodeIdentifier = "pe-1"}
                });

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            refunds.All(refund => refund.AmountDue == -10).Should().BeTrue();
            refunds.All(refund => refund is CalculatedRequiredLevyAmount).Should().BeTrue();
        }
    }
}