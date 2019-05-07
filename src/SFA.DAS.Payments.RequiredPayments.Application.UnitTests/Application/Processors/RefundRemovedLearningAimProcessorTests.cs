using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Application.Infrastructure.Mapping;
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
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RequiredPaymentsProfile>());
            config.AssertConfigurationIsValid();
            var mapper = new Mapper(config);
            mocker.Provide<IMapper>(mapper);
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
                    ReferenceNumber = "learner-ref-123",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learning-ref-456",
                    StandardCode = 6
                },
                Ukprn = 7
            };
            var historicPayment = new PaymentHistoryEntity
            {
                Amount = 10,
                SfaContributionPercentage = .9M,
                TransactionType = (int) OnProgrammeEarningType.Learning,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                LearnAimReference = "aim-ref-123",
                LearnerReferenceNumber = "learning-ref-456",
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = 1,
                Ukprn = 7,
                ActualEndDate = null,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                ExternalId = Guid.NewGuid(),
                FundingSource = FundingSourceType.Levy,
                InstalmentAmount = 1000,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddMonths(-12),
            };
            history.Add(historicPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));
            mocker.Mock<IRefundRemovedLearningAimService>()
                .Setup(x => x.RefundLearningAim(It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>
                {
                    new RequiredPayment{Amount = -10, SfaContributionPercentage = .95M, EarningType = EarningType.Levy, PriceEpisodeIdentifier = "pe-1"}
                });
            mocker.Mock<IPeriodisedRequiredPaymentEventFactory>()
                .Setup(x => x.Create(It.IsAny<EarningType>(), It.IsAny<int>()))
                .Returns(new CalculatedRequiredLevyAmount
                {
                    OnProgrammeEarningType = OnProgrammeEarningType.Learning,
                });

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            refund.AmountDue.Should().Be(-10);
            refund.Should().BeOfType<CalculatedRequiredLevyAmount>();
            refund.EarningEventId.Should().Be(Guid.Empty);
            refund.AccountId.Should().Be(historicPayment.AccountId);
            refund.EventId.Should().NotBe(historicPayment.ExternalId);
            refund.EventId.Should().NotBe(Guid.Empty);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.CollectionPeriod.AcademicYear.Should().Be(identifiedLearner.CollectionPeriod.AcademicYear);
            refund.CollectionPeriod.Period.Should().Be(identifiedLearner.CollectionPeriod.Period);
            refund.CompletionAmount.Should().Be(historicPayment.CompletionAmount);
            refund.ContractType.Should().Be(historicPayment.ContractType);
            refund.CompletionStatus.Should().Be(historicPayment.CompletionStatus);
            refund.DeliveryPeriod.Should().Be(historicPayment.DeliveryPeriod);
            refund.IlrSubmissionDateTime.Should().Be(identifiedLearner.IlrSubmissionDateTime);
            refund.InstalmentAmount.Should().Be(historicPayment.InstalmentAmount);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.JobId.Should().Be(identifiedLearner.JobId);
            refund.Learner.ReferenceNumber.Should().Be(identifiedLearner.Learner.ReferenceNumber);
            refund.Learner.Uln.Should().Be(identifiedLearner.Learner.Uln);
            refund.LearningAim.Reference.Should().Be(identifiedLearner.LearningAim.Reference);
            refund.LearningAim.FrameworkCode.Should().Be(identifiedLearner.LearningAim.FrameworkCode);
            refund.LearningAim.FundingLineType.Should().Be(identifiedLearner.LearningAim.FundingLineType);
            refund.LearningAim.PathwayCode.Should().Be(identifiedLearner.LearningAim.PathwayCode);
            refund.LearningAim.ProgrammeType.Should().Be(identifiedLearner.LearningAim.ProgrammeType);
            refund.LearningAim.StandardCode.Should().Be(identifiedLearner.LearningAim.StandardCode);

        }

        [Test]
        public async Task Refunds_Required_CoInvested_Payments()
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
                    ReferenceNumber = "learner-ref-123",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learning-ref-456",
                    StandardCode = 6
                },
                Ukprn = 7
            };
            var historicPayment = new PaymentHistoryEntity
            {
                Amount = 10,
                SfaContributionPercentage = .9M,
                TransactionType = (int) OnProgrammeEarningType.Learning,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                LearnAimReference = "aim-ref-123",
                LearnerReferenceNumber = "learning-ref-456",
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = 1,
                Ukprn = 7,
                ActualEndDate = null,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                ExternalId = Guid.NewGuid(),
                FundingSource = FundingSourceType.CoInvestedSfa,
                InstalmentAmount = 1000,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddMonths(-12),
            };
            history.Add(historicPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));
            mocker.Mock<IRefundRemovedLearningAimService>()
                .Setup(x => x.RefundLearningAim(It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>
                {
                    new RequiredPayment{Amount = -10, SfaContributionPercentage = .95M, EarningType = EarningType.CoInvested, PriceEpisodeIdentifier = "pe-1"}
                });
            mocker.Mock<IPeriodisedRequiredPaymentEventFactory>()
                .Setup(x => x.Create(It.IsAny<EarningType>(), It.IsAny<int>()))
                .Returns(new CalculatedRequiredCoInvestedAmount
                {
                    OnProgrammeEarningType = OnProgrammeEarningType.Learning
                });

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            refund.AmountDue.Should().Be(-10);
            refund.Should().BeOfType<CalculatedRequiredCoInvestedAmount>();
            refund.EarningEventId.Should().Be(Guid.Empty);
            refund.AccountId.Should().Be(historicPayment.AccountId);
            refund.EventId.Should().NotBe(historicPayment.ExternalId);
            refund.EventId.Should().NotBe(Guid.Empty);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.CollectionPeriod.AcademicYear.Should().Be(identifiedLearner.CollectionPeriod.AcademicYear);
            refund.CollectionPeriod.Period.Should().Be(identifiedLearner.CollectionPeriod.Period);
            refund.CompletionAmount.Should().Be(historicPayment.CompletionAmount);
            refund.ContractType.Should().Be(historicPayment.ContractType);
            refund.CompletionStatus.Should().Be(historicPayment.CompletionStatus);
            refund.DeliveryPeriod.Should().Be(historicPayment.DeliveryPeriod);
            refund.IlrSubmissionDateTime.Should().Be(identifiedLearner.IlrSubmissionDateTime);
            refund.InstalmentAmount.Should().Be(historicPayment.InstalmentAmount);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.JobId.Should().Be(identifiedLearner.JobId);
            refund.Learner.ReferenceNumber.Should().Be(identifiedLearner.Learner.ReferenceNumber);
            refund.Learner.Uln.Should().Be(identifiedLearner.Learner.Uln);
            refund.LearningAim.Reference.Should().Be(identifiedLearner.LearningAim.Reference);
            refund.LearningAim.FrameworkCode.Should().Be(identifiedLearner.LearningAim.FrameworkCode);
            refund.LearningAim.FundingLineType.Should().Be(identifiedLearner.LearningAim.FundingLineType);
            refund.LearningAim.PathwayCode.Should().Be(identifiedLearner.LearningAim.PathwayCode);
            refund.LearningAim.ProgrammeType.Should().Be(identifiedLearner.LearningAim.ProgrammeType);
            refund.LearningAim.StandardCode.Should().Be(identifiedLearner.LearningAim.StandardCode);

        }

[Test]
        public async Task Refunds_Required_Incentive_Payments()
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
                    ReferenceNumber = "learner-ref-123",
                    Uln = 2
                },
                LearningAim = new LearningAim
                {
                    FrameworkCode = 3,
                    FundingLineType = "funding line type",
                    PathwayCode = 4,
                    ProgrammeType = 5,
                    Reference = "learning-ref-456",
                    StandardCode = 6
                },
                Ukprn = 7
            };
            var historicPayment = new PaymentHistoryEntity
            {
                Amount = 10,
                SfaContributionPercentage = .9M,
                TransactionType = (int) IncentiveEarningType.Balancing16To18FrameworkUplift,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = 1819,
                    Period = 1
                },
                LearnAimReference = "aim-ref-123",
                LearnerReferenceNumber = "learning-ref-456",
                PriceEpisodeIdentifier = "pe-1",
                DeliveryPeriod = 1,
                Ukprn = 7,
                ActualEndDate = null,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                ExternalId = Guid.NewGuid(),
                FundingSource = FundingSourceType.FullyFundedSfa,
                InstalmentAmount = 1000,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.Today,
                StartDate = DateTime.Today.AddMonths(-12),
            };
            history.Add(historicPayment);
            mocker.Mock<IDataCache<PaymentHistoryEntity[]>>()
                .Setup(x => x.TryGet(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ConditionalValue<PaymentHistoryEntity[]>(true, history.ToArray()));
            mocker.Mock<IRefundRemovedLearningAimService>()
                .Setup(x => x.RefundLearningAim(It.IsAny<List<Payment>>()))
                .Returns(new List<RequiredPayment>
                {
                    new RequiredPayment{Amount = -10, SfaContributionPercentage = .95M, EarningType = EarningType.Incentive, PriceEpisodeIdentifier = "pe-1"}
                });
            mocker.Mock<IPeriodisedRequiredPaymentEventFactory>()
                .Setup(x => x.Create(It.IsAny<EarningType>(), It.IsAny<int>()))
                .Returns(new CalculatedRequiredIncentiveAmount
                {
                    Type = IncentivePaymentType.Balancing16To18FrameworkUplift
                });

            var processor = mocker.Create<RefundRemovedLearningAimProcessor>();
            var refunds = await processor.RefundLearningAim(identifiedLearner, mocker.Mock<IDataCache<PaymentHistoryEntity[]>>().Object, CancellationToken.None).ConfigureAwait(false);
            refunds.Count.Should().Be(1);
            var refund = refunds.First();
            refund.AmountDue.Should().Be(-10);
            refund.Should().BeOfType<CalculatedRequiredIncentiveAmount>();
            refund.EarningEventId.Should().Be(Guid.Empty);
            refund.AccountId.Should().Be(historicPayment.AccountId);
            refund.EventId.Should().NotBe(historicPayment.ExternalId);
            refund.EventId.Should().NotBe(Guid.Empty);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.CollectionPeriod.AcademicYear.Should().Be(identifiedLearner.CollectionPeriod.AcademicYear);
            refund.CollectionPeriod.Period.Should().Be(identifiedLearner.CollectionPeriod.Period);
            refund.CompletionAmount.Should().Be(historicPayment.CompletionAmount);
            refund.ContractType.Should().Be(historicPayment.ContractType);
            refund.CompletionStatus.Should().Be(historicPayment.CompletionStatus);
            refund.DeliveryPeriod.Should().Be(historicPayment.DeliveryPeriod);
            refund.IlrSubmissionDateTime.Should().Be(identifiedLearner.IlrSubmissionDateTime);
            refund.InstalmentAmount.Should().Be(historicPayment.InstalmentAmount);
            refund.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            refund.JobId.Should().Be(identifiedLearner.JobId);
            refund.Learner.ReferenceNumber.Should().Be(identifiedLearner.Learner.ReferenceNumber);
            refund.Learner.Uln.Should().Be(identifiedLearner.Learner.Uln);
            refund.LearningAim.Reference.Should().Be(identifiedLearner.LearningAim.Reference);
            refund.LearningAim.FrameworkCode.Should().Be(identifiedLearner.LearningAim.FrameworkCode);
            refund.LearningAim.FundingLineType.Should().Be(identifiedLearner.LearningAim.FundingLineType);
            refund.LearningAim.PathwayCode.Should().Be(identifiedLearner.LearningAim.PathwayCode);
            refund.LearningAim.ProgrammeType.Should().Be(identifiedLearner.LearningAim.ProgrammeType);
            refund.LearningAim.StandardCode.Should().Be(identifiedLearner.LearningAim.StandardCode);

        }

    }
}