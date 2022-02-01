using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    [TestFixture]
    public class PeriodEndSummaryTests
    {
        private PeriodEndProviderSummary GetPopulatedPeriodEndProviderSummary(bool shouldIncludeNegativeEarnings = false)
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

        private ProviderPeriodEndSummaryModel GetSubmissionSummaryMetrics(bool shouldIncludeNegativeEarnings = false)
        {
            var summary = GetPopulatedPeriodEndProviderSummary(shouldIncludeNegativeEarnings);
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
            metrics.AdjustedDataLockedEarnings16To18.Should().Be(noSummaries * providerMetricsSummary.AdjustedDataLockedEarnings16To18);
            metrics.AdjustedDataLockedEarnings19Plus.Should().Be(noSummaries * providerMetricsSummary.AdjustedDataLockedEarnings19Plus);

            metrics.AlreadyPaidDataLockedEarnings.Should().Be(noSummaries * providerMetricsSummary.AlreadyPaidDataLockedEarnings);
            metrics.AlreadyPaidDataLockedEarnings16To18.Should().Be(noSummaries * providerMetricsSummary.AlreadyPaidDataLockedEarnings16To18);
            metrics.AlreadyPaidDataLockedEarnings19Plus.Should().Be(noSummaries * providerMetricsSummary.AlreadyPaidDataLockedEarnings19Plus);

            metrics.TotalDataLockedEarnings.Should().Be(noSummaries * providerMetricsSummary.TotalDataLockedEarnings);
            metrics.TotalDataLockedEarnings16To18.Should().Be(noSummaries * providerMetricsSummary.TotalDataLockedEarnings16To18);
            metrics.TotalDataLockedEarnings19Plus.Should().Be(noSummaries * providerMetricsSummary.TotalDataLockedEarnings19Plus);

            metrics.PaymentMetrics.DifferenceContractType1.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceContractType1);
            metrics.PaymentMetrics.DifferenceContractType2.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceContractType2);
            metrics.PaymentMetrics.DifferenceTotal.Should()
                .Be(noSummaries * providerMetricsSummary.PaymentMetrics.DifferenceTotal);
        }

        [TestCase(99.90d, false)]
        [TestCase(100.09d, false)]
        [TestCase(99.99d, true)]
        [TestCase(99.92d, true)]
        [TestCase(100.08d, true)]
        public void WhenCalculatingIsWithinTolerance_ThenIsWithinToleranceSetCorrectly(decimal percentage, bool expectedIsWithinTolerance)
        {
            //Arrange
            var summary = TestHelper.DefaultPeriodEndSummary;
            var model = summary.GetMetrics();
            model.Percentage = percentage;

            //Act
            summary.CalculateIsWithinTolerance(null, null);

            //Assert
            model.IsWithinTolerance.Should().Be(expectedIsWithinTolerance);
        }

        [Test]
        public void WhenGettingMetrics_AndThereAreNegativeEarnings_ThenNegativeEarningsPropertiesAreCorrectlyPopulated()
        {
            //Arrange
            var periodEndSummary = TestHelper.DefaultPeriodEndSummary;
            var providerSummariesList = new List<ProviderPeriodEndSummaryModel>();

            var providerSummary = GetPopulatedPeriodEndProviderSummary(true);
            providerSummariesList.Add(providerSummary.GetMetrics());

            periodEndSummary.AddProviderSummaries(providerSummariesList);

            //Act
            var result = periodEndSummary.GetMetrics();

            //Assert
            result.NegativeEarnings.ContractType1
                .Should()
                .Be(providerSummariesList.Sum(x => x.NegativeEarnings.ContractType1));

            result.NegativeEarnings.ContractType2
                .Should()
                .Be(providerSummariesList.Sum(x => x.NegativeEarnings.ContractType2));
        }
    }
}