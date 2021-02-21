using System;
using Autofac.Extras.Moq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class FundingSourcePaymentEventToProviderPaymentModelMappingTests
    {

        private AutoMock mocker;

        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<ProviderPaymentsProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }

        [SetUp]
        public void SetUpEachTest()
        {
            mocker = AutoMock.GetLoose();
            mocker.Provide<IMapper>(Mapper.Instance);
            //mocker.Provide<IPaymentsLogger>()
        }

        private ProviderPaymentEventModel Map(FundingSourcePaymentEvent message)
        {
            var mapper = mocker.Create<FundingSourceEventMapper>();
            return mapper.Map(message);
        }

        [Test]
        public void CanMapFromEmployerCoInvestedFundingSourceEventToPayment()
        {
            var employerCoInvested = new EmployerCoInvestedFundingSourcePaymentEvent
            {
                EventId = Guid.NewGuid(),
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
                RequiredPaymentEventId = Guid.NewGuid(),
                AccountId = 123456789,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
            };
            var payment = Map(employerCoInvested);
            payment.Ukprn.Should().Be(employerCoInvested.Ukprn);
            payment.CollectionPeriod.Should().Be(employerCoInvested.CollectionPeriod.Period);
            payment.AcademicYear.Should().Be(employerCoInvested.CollectionPeriod.AcademicYear);
            payment.DeliveryPeriod.Should().Be(employerCoInvested.DeliveryPeriod);
            payment.FundingSourceId.Should().Be(employerCoInvested.EventId);
            payment.ContractType.Should().Be(employerCoInvested.ContractType);
            payment.SfaContributionPercentage.Should().Be(employerCoInvested.SfaContributionPercentage);
            payment.Amount.Should().Be(employerCoInvested.AmountDue);
            payment.FundingSource.Should().Be(employerCoInvested.FundingSourceType);
            payment.JobId.Should().Be(employerCoInvested.JobId);
            payment.IlrSubmissionDateTime.Should().Be(employerCoInvested.IlrSubmissionDateTime);
            payment.AccountId.Should().Be(employerCoInvested.AccountId);
            payment.ApprenticeshipEmployerType.Should().Be(employerCoInvested.ApprenticeshipEmployerType);
            payment.RequiredPaymentEventId.Should().Be(employerCoInvested.RequiredPaymentEventId);
        }

        [Test]
        public void CanMapFromLevyFundingSourcePaymentEventToPayment()
        {
            var agreementId = "AGREEMENT";

            var levy = new LevyFundingSourcePaymentEvent
            {
                EventId = Guid.NewGuid(),
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
                AgreementId = agreementId,
                RequiredPaymentEventId = Guid.NewGuid(),
                AccountId = 123456789,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy,
            };

            var payment = Map(levy);
            payment.Ukprn.Should().Be(levy.Ukprn);
            payment.CollectionPeriod.Should().Be(levy.CollectionPeriod.Period);
            payment.AcademicYear.Should().Be(levy.CollectionPeriod.AcademicYear);
            payment.DeliveryPeriod.Should().Be(levy.DeliveryPeriod);
            payment.FundingSourceId.Should().Be(levy.EventId);
            payment.ContractType.Should().Be(levy.ContractType);
            payment.SfaContributionPercentage.Should().Be(levy.SfaContributionPercentage);
            payment.Amount.Should().Be(levy.AmountDue);
            payment.FundingSource.Should().Be(levy.FundingSourceType);
            payment.JobId.Should().Be(levy.JobId);
            payment.IlrSubmissionDateTime.Should().Be(levy.IlrSubmissionDateTime);
            payment.AccountId.Should().Be(levy.AccountId);
            payment.ApprenticeshipEmployerType.Should().Be(levy.ApprenticeshipEmployerType);
            payment.RequiredPaymentEventId.Should().Be(levy.RequiredPaymentEventId);
        }

        [Test]
        public void CanMapFromTransferFundingSourcePaymentEventToPayment()
        {
            var agreementId = "AGREEMENT";

            var transfer = new TransferFundingSourcePaymentEvent
            {
                EventId = Guid.NewGuid(),
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                Learner = new Learner { ReferenceNumber = "1234-ref", Uln = 123456 },
                TransactionType = TransactionType.Completion,
                Ukprn = 12345,
                ContractType = ContractType.Act1,
                SfaContributionPercentage = 0.9m,
                PriceEpisodeIdentifier = "pe-1",
                JobId = 123,
                AmountDue = 300,
                FundingSourceType = FundingSourceType.CoInvestedEmployer,
                DeliveryPeriod = 12,
                LearningAim = new LearningAim
                {
                    PathwayCode = 12,
                    FrameworkCode = 1245,
                    FundingLineType = "Non-DAS 16-18 Learner",
                    StandardCode = 1209,
                    ProgrammeType = 7890,
                    Reference = "1234567-aim-ref"
                },
                IlrSubmissionDateTime = DateTime.UtcNow,
                EventTime = DateTimeOffset.UtcNow,
                AgreementId = agreementId,
                RequiredPaymentEventId = Guid.NewGuid(),
                AccountId = 123456789,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
            };

            var payment = Map(transfer);
            payment.Ukprn.Should().Be(transfer.Ukprn);
            payment.CollectionPeriod.Should().Be(transfer.CollectionPeriod.Period);
            payment.AcademicYear.Should().Be(transfer.CollectionPeriod.AcademicYear);
            payment.DeliveryPeriod.Should().Be(transfer.DeliveryPeriod);
            payment.FundingSourceId.Should().Be(transfer.EventId);
            payment.ContractType.Should().Be(transfer.ContractType);
            payment.SfaContributionPercentage.Should().Be(transfer.SfaContributionPercentage);
            payment.Amount.Should().Be(transfer.AmountDue);
            payment.FundingSource.Should().Be(transfer.FundingSourceType);
            payment.JobId.Should().Be(transfer.JobId);
            payment.IlrSubmissionDateTime.Should().Be(transfer.IlrSubmissionDateTime);
            payment.AccountId.Should().Be(transfer.AccountId);
            payment.ApprenticeshipEmployerType.Should().Be(transfer.ApprenticeshipEmployerType);
            payment.RequiredPaymentEventId.Should().Be(transfer.RequiredPaymentEventId);

        }

        [Test]
        [TestCase(typeof(EmployerCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(SfaCoInvestedFundingSourcePaymentEvent))]
        [TestCase(typeof(SfaFullyFundedFundingSourcePaymentEvent))]
        [TestCase(typeof(LevyFundingSourcePaymentEvent))]
        public void EventModelEarningsInfoShouldBeCorrect(Type fundingSourceEventType)
        {
            var fundingSourceEvent = Activator.CreateInstance(fundingSourceEventType) as FundingSourcePaymentEvent;

            fundingSourceEvent.StartDate = DateTime.UtcNow;
            fundingSourceEvent.PlannedEndDate = DateTime.UtcNow;
            fundingSourceEvent.ActualEndDate = DateTime.UtcNow;
            fundingSourceEvent.CompletionStatus = 3;
            fundingSourceEvent.CompletionAmount = 100M;
            fundingSourceEvent.InstalmentAmount = 200M;
            fundingSourceEvent.NumberOfInstalments = 5;
            fundingSourceEvent.ApprenticeshipId = 800L;
            fundingSourceEvent.ApprenticeshipPriceEpisodeId = 1600L;
            fundingSourceEvent.ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy;
            fundingSourceEvent.LearningAim = new LearningAim
            {
                PathwayCode = 12,
                FrameworkCode = 1245,
                FundingLineType = "Non-DAS 16-18 Learner",
                StandardCode = 1209,
                ProgrammeType = 7890,
                Reference = "1234567-aim-ref"
            };

            var providerPayment = Map(fundingSourceEvent);

            providerPayment.StartDate.Should().Be(fundingSourceEvent.StartDate);
            providerPayment.PlannedEndDate.Should().Be(fundingSourceEvent.PlannedEndDate);
            providerPayment.ActualEndDate.Should().Be(fundingSourceEvent.ActualEndDate);
            providerPayment.CompletionStatus.Should().Be(fundingSourceEvent.CompletionStatus);
            providerPayment.CompletionAmount.Should().Be(fundingSourceEvent.CompletionAmount);
            providerPayment.InstalmentAmount.Should().Be(fundingSourceEvent.InstalmentAmount);
            providerPayment.NumberOfInstalments.Should().Be(fundingSourceEvent.NumberOfInstalments);
            providerPayment.ApprenticeshipId.Should().Be(fundingSourceEvent.ApprenticeshipId);
            providerPayment.ApprenticeshipPriceEpisodeId.Should().Be(fundingSourceEvent.ApprenticeshipPriceEpisodeId);
            providerPayment.ApprenticeshipEmployerType.Should().Be(fundingSourceEvent.ApprenticeshipEmployerType);
            providerPayment.RequiredPaymentEventId.Should().Be(fundingSourceEvent.RequiredPaymentEventId);
        }

    }
}