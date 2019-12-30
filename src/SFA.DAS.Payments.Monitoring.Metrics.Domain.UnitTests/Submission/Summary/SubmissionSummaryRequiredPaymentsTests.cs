using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionSummaryRequiredPaymentsTests
    {
        private SubmissionSummary GetSubmissionSummary => TestsHelper.DefaultSubmissionSummary;
        

        [Test]
        public void Calculates_Correct_Metrics_For_RequiredPayments()
        {
            var summary = GetSubmissionSummary;
            var requiredPayments = TestsHelper.DefaultRequiredPayments;
            summary.AddRequiredPayments(requiredPayments);
            var metrics = summary.GetMetrics();
            metrics.RequiredPaymentsMetrics.Count.Should().Be(2);
            metrics.RequiredPaymentsMetrics.FirstOrDefault(x => x.Amounts.ContractType == ContractType.Act1).Amounts
                .Total.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act1).Total);
            metrics.RequiredPaymentsMetrics.FirstOrDefault(x => x.Amounts.ContractType == ContractType.Act2).Amounts
                .Total.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act2).Total);
        }

        [Test]
        public void Calculates_Correct_Contract_Types_For_Required_Payments()
        {
            var summary = GetSubmissionSummary;
            var requiredPayments = TestsHelper.DefaultRequiredPayments;
            summary.AddRequiredPayments(requiredPayments);
            var metrics = summary.GetMetrics();
            metrics.RequiredPaymentsMetrics.Count.Should().Be(2);
            metrics.RequiredPayments.ContractType1.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act1).Total);
            metrics.RequiredPayments.ContractType2.Should().Be(requiredPayments.FirstOrDefault(x => x.ContractType == ContractType.Act2).Total);
        }
    }
}