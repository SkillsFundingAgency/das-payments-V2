using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionSummaryDataLockedEarningsTests
    {
        private SubmissionSummary GetSubmissionSummary => TestsHelper.DefaultSubmissionSummary;

        [Test]
        public void Calculates_Correct_DataLocked_Earnings_Totals()
        {
            var summary = GetSubmissionSummary;
            summary.AddDataLockedEarnings(TestsHelper.DefaultDataLockedEarnings.Total, TestsHelper.DefaultDataLockedEarnings);
            var metrics = summary.GetMetrics();
            metrics.DataLockedEarnings.Should().Be(TestsHelper.DefaultDataLockedEarnings.Total);
            metrics.DataLockedPaymentsMetrics.Count.Should().Be(1);
            metrics.DataLockedPaymentsMetrics.Sum(x => x.Amounts.Total).Should()
                .Be(TestsHelper.DefaultDataLockedEarnings.Total);
        }
    }
}