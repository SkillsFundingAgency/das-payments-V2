using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionSummaryHeldBackCompletionPaymentsTests
    {
        private SubmissionSummary GetSubmissionSummary => TestsHelper.DefaultSubmissionSummary;

        [Test]
        public void Calculates_Correct_HeldBack_Payments_Totals()
        {
            var summary = GetSubmissionSummary;
            var heldBackAmounts = TestsHelper.DefaultHeldBackCompletionPayments;
            summary.AddHeldBackCompletionPayments(heldBackAmounts);
            var metrics = summary.GetMetrics();
            metrics.HeldBackCompletionPayments.ContractType1.Should().Be( heldBackAmounts.ContractType1);
            metrics.HeldBackCompletionPayments.ContractType2.Should().Be(heldBackAmounts.ContractType2);
            metrics.HeldBackCompletionPayments.Total.Should().Be(heldBackAmounts.Total);
        }
    }
}