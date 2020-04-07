using System;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Mapping;
using SFA.DAS.Payments.ProviderPayments.Messages;

namespace SFA.DAS.Payments.ProviderPayments.Application.UnitTests.Mapping
{
    [TestFixture]
    public class RecordedAct1CompletionPaymentEventMappingTests
    {

        [OneTimeSetUp]
        public void SetUp()
        {
            Mapper.Reset();
            Mapper.Initialize(cfg => { cfg.AddProfile<ProviderPaymentsProfile>(); });
            Mapper.AssertConfigurationIsValid();
        }

        [Test]
        public void CanMapFromPaymentModelToRecordedAct1CompletionPaymentEvent()
        {
            var paymentModel = new PaymentModel
            {
                JobId = 123,
                EventId = Guid.NewGuid(),
                EventTime = DateTimeOffset.UtcNow,
                Ukprn = 12345,
                DeliveryPeriod = 12,
                CollectionPeriod = new CollectionPeriod { Period = 12, AcademicYear = 1819 },
                LearnerUln = 123456,
                LearnerReferenceNumber = "1234-ref",
                PriceEpisodeIdentifier = "pe-1",
                LearningAimPathwayCode = 12,
                LearningAimFrameworkCode = 1245,
                LearningAimFundingLineType = "Non-DAS 16-18 Learner",
                LearningAimStandardCode = 1209,
                LearningAimProgrammeType = 7890,
                LearningAimReference = "1234567-aim-ref",
                IlrSubmissionDateTime = DateTime.UtcNow,
                TransactionType = TransactionType.Completion,
                SfaContributionPercentage = 0.9m,
                FundingSource = FundingSourceType.CoInvestedEmployer,
                Amount = 300,
                AccountId = 123456789,
                TransferSenderAccountId = 123456789,
                CompletionAmount = 3000,
                CompletionStatus = 1,
                InstalmentAmount = 100,
                StartDate = DateTime.UtcNow,
                ActualEndDate = DateTime.UtcNow,
                NumberOfInstalments = 12,
                PlannedEndDate = DateTime.UtcNow,
                ApprenticeshipId = 800L,
                ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy,
                ReportingAimFundingLineType = "ReportingAimFundingLineType",
                ContractType = ContractType.Act1,

            };

            var payment = Mapper.Map<PaymentModel, RecordedAct1CompletionPayment>(paymentModel);
            
            payment.EventId.Should().NotBeEmpty();
            payment.EventTime.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(5));
            payment.Ukprn.Should().Be(paymentModel.Ukprn);
            payment.DeliveryPeriod.Should().Be(paymentModel.DeliveryPeriod);
            payment.CollectionPeriod.Period.Should().Be(paymentModel.CollectionPeriod.Period);
            payment.CollectionPeriod.AcademicYear.Should().Be(paymentModel.CollectionPeriod.AcademicYear);
            payment.Learner.Uln.Should().Be(paymentModel.LearnerUln);
            payment.Learner.ReferenceNumber.Should().Be(paymentModel.LearnerReferenceNumber);
            payment.LearningAim.Reference.Should().Be(paymentModel.LearningAimReference);
            payment.LearningAim.FrameworkCode.Should().Be(paymentModel.LearningAimFrameworkCode);
            payment.LearningAim.PathwayCode.Should().Be(paymentModel.LearningAimPathwayCode);
            payment.LearningAim.ProgrammeType.Should().Be(paymentModel.LearningAimProgrammeType);
            payment.LearningAim.StandardCode.Should().Be(paymentModel.LearningAimStandardCode);
            payment.LearningAim.FundingLineType.Should().Be(paymentModel.LearningAimFundingLineType);
            payment.LearningAim.StartDate.Should().Be(paymentModel.LearningStartDate.GetValueOrDefault());
            payment.IlrSubmissionDateTime.Should().Be(paymentModel.IlrSubmissionDateTime);
            payment.TransactionType.Should().Be(paymentModel.TransactionType);
            payment.SfaContributionPercentage.Should().Be(paymentModel.SfaContributionPercentage);
            payment.FundingSource.Should().Be(paymentModel.FundingSource);
            payment.AmountDue.Should().Be(paymentModel.Amount);
            payment.AccountId.Should().Be(paymentModel.AccountId);
            payment.TransferSenderAccountId.Should().Be(paymentModel.TransferSenderAccountId);
            payment.EarningDetails.CompletionAmount.Should().Be(paymentModel.CompletionAmount);
            payment.EarningDetails.CompletionStatus.Should().Be(paymentModel.CompletionStatus);
            payment.EarningDetails.InstalmentAmount.Should().Be(paymentModel.InstalmentAmount);
            payment.EarningDetails.StartDate.Should().Be(paymentModel.StartDate);
            payment.EarningDetails.ActualEndDate.Should().Be(paymentModel.ActualEndDate);
            payment.EarningDetails.NumberOfInstalments.Should().Be(paymentModel.NumberOfInstalments);
            payment.EarningDetails.PlannedEndDate.Should().Be(paymentModel.PlannedEndDate);
            payment.ApprenticeshipId.Should().Be(paymentModel.ApprenticeshipId);
            payment.ApprenticeshipEmployerType.Should().Be(paymentModel.ApprenticeshipEmployerType);
            payment.ReportingAimFundingLineType.Should().Be(paymentModel.ReportingAimFundingLineType);
            payment.ContractType.Should().Be(paymentModel.ContractType);
            payment.JobId.Should().NotBe(paymentModel.JobId);
        }
    }
}