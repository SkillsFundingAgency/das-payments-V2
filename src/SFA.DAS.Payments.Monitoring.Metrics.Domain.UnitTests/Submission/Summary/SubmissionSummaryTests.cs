using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.Submission;
using SFA.DAS.Payments.Monitoring.Metrics.Model;
using SFA.DAS.Payments.Monitoring.Metrics.Model.Submission;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.Submission.Summary
{
    [TestFixture]
    public class SubmissionSummaryTests
    {
        private List<TransactionTypeAmounts> dcEarnings;
        private List<TransactionTypeAmounts> dasEarnings;
        private List<TransactionTypeAmounts> requiredPayments;
        private DataLockTypeAmounts dataLocks;
        private ContractTypeAmounts heldBackCompletionPayments;
        private decimal totalDataLockedEarnings;

        private SubmissionSummary GetSubmissionSummary()
        {
            dcEarnings = TestsHelper.DefaultDcEarnings;
            dasEarnings = TestsHelper.DefaultDasEarnings;
            dataLocks = TestsHelper.DefaultDataLockedEarnings;
            requiredPayments = TestsHelper.DefaultRequiredPayments;
            heldBackCompletionPayments = TestsHelper.DefaultHeldBackCompletionPayments;
            var summary = TestsHelper.DefaultSubmissionSummary;
            summary.AddEarnings(dcEarnings, dasEarnings);
            summary.AddDataLockedEarnings(TestsHelper.DefaultDataLockedTotal, dataLocks);
            summary.AddRequiredPayments(requiredPayments);
            summary.AddHeldBackCompletionPayments(heldBackCompletionPayments);
            return summary;
        }

        private decimal DcEarningsContractType(ContractType contractType) =>
            dcEarnings.FirstOrDefault(x => x.ContractType == contractType).Total;

        private SubmissionSummaryModel GetSubmissionSummaryMetrics()
        {
            var summary = GetSubmissionSummary();
            return summary.GetMetrics();
        }

        [Test]
        public void Calculates_Correct_ContractTypes()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.SubmissionMetrics.ContractType1.Should().Be(
                requiredPayments.GetTotal(ContractType.Act1) + TestsHelper.DefaultDataLockedTotal +
                heldBackCompletionPayments.ContractType1);
            metrics.SubmissionMetrics.ContractType2.Should().Be(
                requiredPayments.GetTotal(ContractType.Act2) +
                heldBackCompletionPayments.ContractType2);
        }


        [Test]
        public void Calculates_Correct_ContractType_Percentages()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.SubmissionMetrics.PercentageContractType1.Should().Be(100);
            metrics.SubmissionMetrics.PercentageContractType2.Should().Be(100);
        }


        [Test]
        public void Calculates_Correct_Percentage()
        {
            var metrics = GetSubmissionSummaryMetrics();
            metrics.Percentage.Should().Be(100);
        }
    }
}