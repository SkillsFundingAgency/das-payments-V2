using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    [TestFixture]
    public class PeriodEndSummaryTests
    {
        
        private PeriodEndProviderSummary GetPopulatedPeriodEndProviderSummary()
        {
            var summary = TestHelper.DefaultPeriodEndProviderSummary;
            summary.AddDcEarnings(TestHelper.GetDefaultDcEarnings);
            var yearToDateAmounts = TestHelper.DefaultYearToDateAmounts;
            summary.AddPaymentsYearToDate(yearToDateAmounts);
            var alreadyPaidDataLockedEarnings = TestHelper.AlreadyPaidDataLockedEarnings;
            summary.AddDataLockedAlreadyPaid(alreadyPaidDataLockedEarnings);
            var dataLockedEarningsTotal = TestHelper.DefaultDataLockedTotal;
            summary.AddDataLockedEarnings(dataLockedEarningsTotal);
            summary.AddTransactionTypes(TestHelper.GetPaymentTransactionTypes);
            summary.AddFundingSourceAmounts(TestHelper.GetPaymentFundingSourceAmounts);
            summary.AddHeldBackCompletionPayments(TestHelper.DefaultHeldBackCompletionPayments);
            summary.AddPeriodEndProviderDataLockTypeCounts(TestHelper.DefaultPeriodEndProviderDataLockTypeCounts);
            return summary;
        }

        private ProviderPeriodEndSummaryModel GetSubmissionSummaryMetrics()
        {
            var summary = GetPopulatedPeriodEndProviderSummary();
            return summary.GetMetrics();
        }

        [Test]
        public void GetMetrics_GivenEmptyProviderSummaries_ShouldHaveEmptyTotals()
        {
            var summary = TestHelper.DefaultPeriodEndSummary;
            summary.AddProviderSummaries(new List<ProviderPeriodEndSummaryModel>());
            var metrics = summary.GetMetrics();
            metrics.DcEarnings.Total.Should().Be(0m);
            metrics.HeldBackCompletionPayments.Total.Should().Be(0m);
            metrics.Payments.Total.Should().Be(0m);
            metrics.PaymentMetrics.DifferenceContractType1.Should().Be(0m);
            metrics.PaymentMetrics.DifferenceContractType2.Should().Be(0m);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(1500)]
        public void GetMetrics_AggregatesCorrectly_Given_ProviderLevel_SummaryMetrics(int noSummaries)
        {
            var summary = TestHelper.DefaultPeriodEndSummary;
            var models = new List<ProviderPeriodEndSummaryModel>();
            
            var providerMetricsSummary = GetSubmissionSummaryMetrics();
            for (int i = 0; i < noSummaries; i++)
            {
                models.Add(providerMetricsSummary);
            }

            summary.AddProviderSummaries(models);
            var metrics = summary.GetMetrics();

            metrics.DcEarnings.ContractType1.Should().Be(noSummaries * providerMetricsSummary.DcEarnings.ContractType1);
            metrics.DcEarnings.ContractType2.Should().Be(noSummaries * providerMetricsSummary.DcEarnings.ContractType2);
            metrics.DcEarnings.Total.Should().Be(noSummaries * providerMetricsSummary.DcEarnings.Total);

            metrics.HeldBackCompletionPayments.ContractType1.Should().Be(noSummaries * providerMetricsSummary.HeldBackCompletionPayments.ContractType1);
            metrics.HeldBackCompletionPayments.ContractType2.Should().Be(noSummaries * providerMetricsSummary.HeldBackCompletionPayments.ContractType2);
            metrics.HeldBackCompletionPayments.Total.Should().Be(noSummaries * providerMetricsSummary.HeldBackCompletionPayments.Total);
            
            metrics.Payments.ContractType1.Should().Be(noSummaries * providerMetricsSummary.Payments.ContractType1);
            metrics.Payments.ContractType2.Should().Be(noSummaries * providerMetricsSummary.Payments.ContractType2);
            metrics.Payments.Total.Should().Be(noSummaries * providerMetricsSummary.Payments.Total);

            metrics.Percentage.Should().Be(providerMetricsSummary.Percentage);
            metrics.AdjustedDataLockedEarnings.Should().Be(noSummaries * providerMetricsSummary.AdjustedDataLockedEarnings);
            metrics.AlreadyPaidDataLockedEarnings.Should().Be(noSummaries * providerMetricsSummary.AlreadyPaidDataLockedEarnings);

            metrics.PaymentMetrics.DifferenceContractType1.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceContractType1);
            metrics.PaymentMetrics.DifferenceContractType2.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceContractType2);
            metrics.PaymentMetrics.DifferenceTotal.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceTotal);
        }

    }
}