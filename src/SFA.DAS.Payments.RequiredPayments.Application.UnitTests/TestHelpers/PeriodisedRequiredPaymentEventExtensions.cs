using System;
using FluentAssertions;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;
using SFA.DAS.Payments.RequiredPayments.Model.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Application.UnitTests.TestHelpers
{
    public static class PeriodisedRequiredPaymentEventExtensions
    {
        public static void ShouldBeMappedTo(this PeriodisedRequiredPaymentEvent paymentEvent, IdentifiedRemovedLearningAim identifiedLearningAim)
        {
            paymentEvent.EarningEventId.Should().Be(Guid.Empty);
            paymentEvent.EventId.Should().NotBe(Guid.Empty);
            paymentEvent.CollectionPeriod.AcademicYear.Should().Be(identifiedLearningAim.CollectionPeriod.AcademicYear);
            paymentEvent.CollectionPeriod.Period.Should().Be(identifiedLearningAim.CollectionPeriod.Period);
            paymentEvent.IlrSubmissionDateTime.Should().Be(identifiedLearningAim.IlrSubmissionDateTime);
            paymentEvent.JobId.Should().Be(identifiedLearningAim.JobId);
            paymentEvent.Learner.ReferenceNumber.Should().Be(identifiedLearningAim.Learner.ReferenceNumber);
            paymentEvent.LearningAim.Reference.Should().Be(identifiedLearningAim.LearningAim.Reference);
            paymentEvent.LearningAim.FrameworkCode.Should().Be(identifiedLearningAim.LearningAim.FrameworkCode);
            paymentEvent.LearningAim.FundingLineType.Should().Be(identifiedLearningAim.LearningAim.FundingLineType);
            paymentEvent.LearningAim.PathwayCode.Should().Be(identifiedLearningAim.LearningAim.PathwayCode);
            paymentEvent.LearningAim.ProgrammeType.Should().Be(identifiedLearningAim.LearningAim.ProgrammeType);
            paymentEvent.LearningAim.StandardCode.Should().Be(identifiedLearningAim.LearningAim.StandardCode);
        }

        public static void ShouldBeMappedTo(this PeriodisedRequiredPaymentEvent paymentEvent, PaymentHistoryEntity historicPayment)
        {
            paymentEvent.EarningEventId.Should().Be(Guid.Empty);
            paymentEvent.EventId.Should().NotBe(Guid.Empty);
            paymentEvent.AccountId.Should().Be(historicPayment.AccountId);
            paymentEvent.ApprenticeshipEmployerType.Should().Be(historicPayment.ApprenticeshipEmployerType);
            paymentEvent.EventId.Should().NotBe(historicPayment.ExternalId);
            paymentEvent.EventId.Should().NotBe(Guid.Empty);
            paymentEvent.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            paymentEvent.CompletionAmount.Should().Be(historicPayment.CompletionAmount);
            paymentEvent.ContractType.Should().Be(historicPayment.ContractType);
            paymentEvent.CompletionStatus.Should().Be(historicPayment.CompletionStatus);
            paymentEvent.DeliveryPeriod.Should().Be(historicPayment.DeliveryPeriod);
            paymentEvent.InstalmentAmount.Should().Be(historicPayment.InstalmentAmount);
            paymentEvent.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            paymentEvent.Learner.Uln.Should().Be(historicPayment.LearnerUln);
        }

        public static void AssertPeriodisedRequiredPaymentEventWasCorrectlyMappedFrom(this PeriodisedRequiredPaymentEvent dest, PaymentModel source)
        {
            dest.IlrFileName.Should().BeNull();
            dest.AmountDue.Should().Be(source.Amount);
            dest.AccountId.Should().Be(source.AccountId);
            dest.ActualEndDate.Should().Be(source.ActualEndDate);
            dest.ApprenticeshipEmployerType.Should().Be(source.ApprenticeshipEmployerType);
            dest.ApprenticeshipId.Should().Be(source.ApprenticeshipId);
            dest.ApprenticeshipPriceEpisodeId.Should().Be(source.ApprenticeshipPriceEpisodeId);
            dest.CollectionPeriod.AcademicYear.Should().Be(source.CollectionPeriod.AcademicYear);
            dest.CollectionPeriod.Period.Should().Be(source.CollectionPeriod.Period);
            dest.CompletionAmount.Should().Be(source.CompletionAmount);
            dest.CompletionStatus.Should().Be(source.CompletionStatus);
            dest.ContractType.Should().Be(source.ContractType);
            dest.ClawbackSourcePaymentEventId.Should().Be(source.ClawbackSourcePaymentEventId);
            dest.DeliveryPeriod.Should().Be(source.DeliveryPeriod);
            dest.EarningEventId.Should().Be(source.EarningEventId);
            dest.IlrSubmissionDateTime.Should().Be(source.IlrSubmissionDateTime);
            dest.InstalmentAmount.Should().Be(source.InstalmentAmount);
            dest.JobId.Should().Be(source.JobId);
            dest.LearningAim.PathwayCode.Should().Be(source.LearningAimPathwayCode);
            dest.LearningAim.FrameworkCode.Should().Be(source.LearningAimFrameworkCode);
            dest.LearningAim.FundingLineType.Should().Be(source.LearningAimFundingLineType);
            dest.LearningAim.ProgrammeType.Should().Be(source.LearningAimProgrammeType);
            dest.LearningAim.Reference.Should().Be(source.LearningAimReference);
            dest.LearningAim.StandardCode.Should().Be(source.LearningAimStandardCode);
            dest.LearningAim.StartDate.Should().Be(source.StartDate);
            dest.Learner.Uln.Should().Be(source.LearnerUln);
            dest.Learner.ReferenceNumber.Should().Be(source.LearnerReferenceNumber);
            dest.LearningStartDate.Should().Be(source.LearningStartDate);
            dest.NumberOfInstalments.Should().Be(source.NumberOfInstalments);
            dest.PlannedEndDate.Should().Be(source.PlannedEndDate);
            dest.PriceEpisodeIdentifier.Should().Be(source.PriceEpisodeIdentifier);
            dest.ReportingAimFundingLineType.Should().Be(source.ReportingAimFundingLineType);
            dest.StartDate.Should().Be(source.StartDate);
            dest.TransferSenderAccountId.Should().Be(source.TransferSenderAccountId);
            dest.Ukprn.Should().Be(source.Ukprn);
        }

        public static void AssertCalculatedRequiredLevyAmountWasCorrectlyMappedFrom(this CalculatedRequiredLevyAmount dest, PaymentModel source)
        {
            dest.AssertPeriodisedRequiredPaymentEventWasCorrectlyMappedFrom(source);

            dest.AgreedOnDate.Should().BeNull();
            dest.Priority.Should().Be(default);
            dest.AgreementId.Should().Be(source.AgreementId);
            dest.SfaContributionPercentage.Should().Be(source.SfaContributionPercentage);
            dest.TransactionType.Should().Be(default(TransactionType));
            dest.OnProgrammeEarningType.Should().Be(default(OnProgrammeEarningType));
        }

        public static void AssertCalculatedRequiredCoInvestedAmountWasCorrectlyMappedFrom(this CalculatedRequiredCoInvestedAmount dest, PaymentModel source)
        {
            dest.AssertPeriodisedRequiredPaymentEventWasCorrectlyMappedFrom(source);

            dest.SfaContributionPercentage.Should().Be(source.SfaContributionPercentage);
            dest.TransactionType.Should().Be(default(TransactionType));
            dest.OnProgrammeEarningType.Should().Be(default(OnProgrammeEarningType));
        }
    }
}