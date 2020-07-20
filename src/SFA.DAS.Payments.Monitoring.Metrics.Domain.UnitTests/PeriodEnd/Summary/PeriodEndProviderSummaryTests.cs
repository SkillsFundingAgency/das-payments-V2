using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Payments.Monitoring.Metrics.Domain.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Metrics.Domain.UnitTests.PeriodEnd.Summary
{
    [TestFixture]
    public class PeriodEndProviderSummaryTests
    {
        protected PeriodEndProviderSummary GetSubmissionSummary => TestHelper.DefaultPeriodEndProviderSummary;

        [SetUp]
        public void SetUp()
        {
            
        }

        [Test]
        public void Calculates_Correct_Year_To_Date_Payments_Totals()
        {
            var summary = TestHelper.DefaultPeriodEndProviderSummary;
            var yearToDateAmounts = TestHelper.DefaultYearToDateAmounts;
            summary.AddPaymentsYearToDate(yearToDateAmounts);;
            var metrics = summary.GetMetrics();
            metrics.YearToDatePayments.ContractType1.Should().Be(yearToDateAmounts.ContractType1);
            metrics.YearToDatePayments.ContractType2.Should().Be(yearToDateAmounts.ContractType2);
            metrics.YearToDatePayments.Total.Should().Be(yearToDateAmounts.Total);
        }
        
        [Test]
        public void Calculates_Correct_DataLocked_Earnings_Totals()
        {
           var summary = GetSubmissionSummary;
           var alreadyPaidDataLockedEarnings = TestHelper.AlreadyPaidDataLockedEarnings;
           summary.AddDataLockedAlreadyPaid(alreadyPaidDataLockedEarnings);
           var dataLockedEarningsTotal = TestHelper.DefaultDataLockedTotal;
           summary.AddDataLockedEarnings(dataLockedEarningsTotal);

           var metrics = summary.GetMetrics();
           metrics.AlreadyPaidDataLockedEarnings.Should().Be(alreadyPaidDataLockedEarnings);
           metrics.DataLockedEarnings.Should().Be(dataLockedEarningsTotal);
           metrics.TotalDataLockedEarnings.Should().Be(dataLockedEarningsTotal - alreadyPaidDataLockedEarnings);
        }
        
        [Test]
        public void Calculates_Correct_Metrics_For_DcEarnings()
        {
            var submissionSummary = GetSubmissionSummary;
            var defaultDcEarnings = TestHelper.GetDefaultDcEarnings;
            submissionSummary.AddDcEarning(defaultDcEarnings);
            var metrics = submissionSummary.GetMetrics();
            metrics.DcEarnings.ContractType1.Should().Be(32600);
            metrics.DcEarnings.ContractType2.Should().Be(32600);
            metrics.DcEarnings.Total.Should().Be(65200);
        }
    }
}