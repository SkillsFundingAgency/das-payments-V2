using System;
using FluentAssertions;
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
            paymentEvent.Learner.Uln.Should().Be(identifiedLearningAim.Learner.Uln);
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
            paymentEvent.EventId.Should().NotBe(historicPayment.ExternalId);
            paymentEvent.EventId.Should().NotBe(Guid.Empty);
            paymentEvent.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
            paymentEvent.CompletionAmount.Should().Be(historicPayment.CompletionAmount);
            paymentEvent.ContractType.Should().Be(historicPayment.ContractType);
            paymentEvent.CompletionStatus.Should().Be(historicPayment.CompletionStatus);
            paymentEvent.DeliveryPeriod.Should().Be(historicPayment.DeliveryPeriod);
            paymentEvent.InstalmentAmount.Should().Be(historicPayment.InstalmentAmount);
            paymentEvent.ActualEndDate.Should().Be(historicPayment.ActualEndDate);
        }
    }
}